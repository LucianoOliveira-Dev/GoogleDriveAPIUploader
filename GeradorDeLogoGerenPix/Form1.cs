using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace GeradorDeLogoGerenPix
{
    public partial class Form1 : Form
    {

        private string      _filePath = string.Empty;
        private long        _currentUploadTotalBytes;
        private const int   MAX_SIZE_MB = 1; // Aqui determina o tamanho maximo da imagem

        private string PARENT_FOLDER_ID => AppConfig.ParentFolderId;

        public Form1()
        {
            InitializeComponent();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Imagens|*.png;*.jpg;*.jpeg";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _filePath = openFileDialog.FileName;
                    FileInfo fileInfo = new FileInfo(_filePath);

                    if (fileInfo.Length > MAX_SIZE_MB * 1024 * 1024)
                    {
                        lblStatus.Text = "Erro: Arquivo maior que 1MB!";
                        btnUpload.Enabled = false;
                        picLogo.Image = null;
                        return;
                    }

                    picLogo.ImageLocation = _filePath;
                    lblStatus.Text = "Pronto para upload!";
                    btnUpload.Enabled = true;
                }
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                MessageBox.Show("Selecione um arquivo primeiro!");
                return;
            }

            if (string.IsNullOrEmpty(txtDocumento.Text))
            {
                MessageBox.Show("Informe o CNPJ/CPF do cliente!");
                return;
            }

            btnUpload.Enabled = false;
            progressBar1.Value = 0;
            lblStatus.Text = "Autenticando...";

            try
            {
                _currentUploadTotalBytes = new FileInfo(_filePath).Length;

                UserCredential credential;
                using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.Scope.DriveFile },
                        "user",
                        CancellationToken.None,
                        new FileDataStore("token.json", true));
                }

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Upload Logo Drive",
                });

                // Verifica se a pasta com o CNPJ já existe ou cria uma nova
                string cnpjFolderId = await GetOrCreateFolder(service, txtDocumento.Text);

                lblStatus.Text = "Enviando para o Google Drive...";
                progressBar1.Style = ProgressBarStyle.Blocks;

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(_filePath),
                    Parents = new[] { cnpjFolderId } // Usa a pasta do CNPJ como parent
                };

                using (var stream = new FileStream(_filePath, FileMode.Open))
                {
                    var request = service.Files.Create(fileMetadata, stream, "image/jpeg");
                    request.Fields = "id, webViewLink";
                    request.ProgressChanged += Upload_ProgressChanged;

                    var progress = await request.UploadAsync();

                    if (progress.Status == UploadStatus.Completed)
                    {
                        var file = request.ResponseBody;
                        lblStatus.Text = "Upload concluído!";
                        progressBar1.Value = 100;
                        MessageBox.Show($"Logo salvo em: {file.WebViewLink}", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblStatus.Text = "Erro: " + progress.Exception.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Erro: " + ex.Message;
                MessageBox.Show($"Falha na autenticação ou upload. Detalhes: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnUpload.Enabled = true;
            }
        }

        private async Task<string> GetOrCreateFolder(DriveService service, string folderName)
        {

            if (string.IsNullOrEmpty(PARENT_FOLDER_ID))
            {
                throw new InvalidOperationException("ParentFolderId não configurado no appsettings.json");
            }  // Verifica se o appSettings esta configurado ********************************

            // verifica se a pasta já existe
            var listRequest = service.Files.List();
            listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and '{PARENT_FOLDER_ID}' in parents and trashed=false";
            listRequest.Fields = "files(id, name)";

            var folders = await listRequest.ExecuteAsync();

            if (folders.Files.Count > 0)
            {
                return folders.Files[0].Id; // Retorna o ID se a pasta existir xD
            }

            // Se não existe, cria uma nova :o
            var folderMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new[] { PARENT_FOLDER_ID }
            };

            var request = service.Files.Create(folderMetadata);
            request.Fields = "id";
            var folder = await request.ExecuteAsync();

            return folder.Id;
        }

        private void Upload_ProgressChanged(IUploadProgress progress)
        {
            this.Invoke((MethodInvoker)delegate
            {
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        int percentage = (int)(progress.BytesSent * 100 / _currentUploadTotalBytes);
                        progressBar1.Value = Math.Min(percentage, 100);
                        lblStatus.Text = $"Enviando... {percentage}%";
                        break;

                    case UploadStatus.Completed:
                        progressBar1.Value = 100;
                        lblStatus.Text = "Upload concluído!";
                        break;

                    case UploadStatus.Failed:
                        progressBar1.Value = 0;
                        lblStatus.Text = "Falha no upload";
                        break;
                }
            });
        }

        public static class AppConfig
        {
            public static IConfigurationRoot Configuration { get; private set; }

            static AppConfig()
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);

                Configuration = builder.Build();
            }

            public static string ParentFolderId => Configuration["DriveSettings:ParentFolderId"];
        }
    }
}