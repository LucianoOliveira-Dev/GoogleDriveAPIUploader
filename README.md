# GoogleDriveAPIUploader

# Programa basico utilizando api do Drive para enviar imagens ao seu Google Drive por meio de autenticação OAuth2.0 

################################# ATENÇÃO LEIA ANTES DE USAR #####################################################
Para voce usar de melhor maneira, voce deve primeiro hablitar o serviço de autenticação do google em sua conta. 
Passo 1 - Acesse: https://console.cloud.google.com/ 
    1.1 - Crie um novo projeto de sua preferencia.
    1.2 - No menu latera esquerdo superior, selecione "APIs e serviços" em seguida "Biblioteca". 
    1.3 - Pesquise "Google Drive API" e instale.
    1.4 - Vá de novo no menu lateral esquerdo superior, selecione agora "APIs e serviços" em seguida "Tela de permissão OAuth".
    1.5 - No menu esquerdo que se abrir, clique em "Clientes" e adicione um cliente selecionando o tipo de aplicativo desenvolvido (no caso "App para computador").
    1.6 - Baixe o JSON que o Google fornecer, modifique o nome dele para "client_secret.json" e coloque no local do seu programa.       
Passo 2 - Crie uma pasta no seu Google Drive e deixa ela aberta para publico. 
Passo 3 - Vá para a pasta raiz do seu programa, e edite "appSettings.json": dentro dele, mude a linha "ParentFolderId": "*COLOQUE O ID DA PASTA AQUI*".
#################################################################################################################

Feito isso, você já deve estar configurado o seu programa. 
