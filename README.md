# GoogleDriveAPIUploader

📌 Um programa básico para upload de imagens no Google Drive utilizando a API Drive com autenticação OAuth 2.0

## 📋 Pré-requisitos
- [Google Cloud Console](https://console.cloud.google.com/) ativo
- Conta no Google Drive
- .NET 6.0 ou superior instalado

## ⚙️ Configuração Inicial

### 1. Configuração na Google Cloud Console
1. Acesse o [Google Cloud Console](https://console.cloud.google.com/)
2. Crie um novo projeto
3. No menu lateral esquerdo:
   - Selecione **"APIs e Serviços" > "Biblioteca"**
   - Pesquise por **"Google Drive API"** e habilite
4. Configure as credenciais:
   - Vá para **"APIs e Serviços" > "Tela de permissão OAuth"**
     - Preencha as informações necessárias (nome do app, email de suporte, etc.)
   - Em **"Credenciais" > "Criar Credenciais"**, selecione:
     - Tipo: **Aplicativo para computador**
     - Nome: `GoogleDriveAPIUploader`
5. Baixe o arquivo JSON de credenciais e renomeie para `client_secret.json`

### 2. Configuração no Google Drive
1. Crie uma pasta no seu Drive
2. Compartilhe com acesso público (opcional):
   - Clique com o botão direito na pasta > "Compartilhar"
   - Selecione "Qualquer pessoa com o link"

### 3. Configuração do App
1. Obtenha o ID da pasta do Drive:
   - Abra a pasta no navegador
   - O ID aparece na URL: `https://drive.google.com/drive/folders/[AQUI_VAI_O_ID]`
2. Edite o arquivo `appsettings.json`:
   ```json
   {
     "DriveSettings": {
       "ParentFolderId": "COLE_AQUI_O_ID_DA_PASTA"
     }
   }
