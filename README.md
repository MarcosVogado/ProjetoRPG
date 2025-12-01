# Guilda RPG - API + WPF

API ASP.NET Core 9 + EF Core 9 (SQLite) com tema de guilda de RPG. Inclui CRUD via controllers, relacionamento N:N (Personagem x Missao) e cliente WPF para consumir a API. Swagger exposto para testes.

## Principais features
- CRUD completo: /api/personagens e /api/missoes, com atribuicao/remocao de personagens em missoes.
- Relacao N:N via PersonagemMissao, carregada nas listagens/detalhes.
- Validacoes com DataAnnotations e respostas 400/404 apropriadas.
- SQLite com migrations; seeder cria dados de exemplo quando o banco esta vazio.
- Swagger/SwaggerUI em /swagger.
- Cliente WPF (GuildaClient) para CRUD e atribuicoes.

## Estrutura
- `ProjetoRPG/` API ASP.NET Core 9
  - `Data/RpgContext.cs` mapeia relacao N:N
  - `Models/*.cs` entidades e enums em portugues
  - `Controllers/PersonagensController.cs`, `Controllers/MissoesController.cs`
  - `Program.cs` configura Swagger, DbContext e seeder
- `GuildaClient/` cliente WPF (net9.0-windows)
- `ProjetoRPG.slnx` referencia API e WPF
- `ProjetoRPG.http` chamadas de teste

## Requisitos
- .NET SDK 9.0.x (`dotnet --list-sdks` deve mostrar 9.0.x)
- Tool `dotnet-ef` 9.0.1
  ```powershell
  dotnet tool uninstall --global dotnet-ef
  dotnet tool install --global dotnet-ef --version 9.0.1
  ```

## Preparar banco (SQLite)
No diretorio `ProjetoRPG`:
```powershell
cd ProjetoRPG
dotnet restore
# usar migration existente
cd ..
```
Ja existe a migration `20251201032425_Inicial` (designer incluso). Para aplicar o schema:
```powershell
cd ProjetoRPG
dotnet ef database update
```
Se houver um `guilda.db` antigo sem tabelas/dados, delete antes (`Remove-Item guilda.db`).

## Seeder
Em `Program.cs` a funcao `SeedDatabase` roda apos `Database.Migrate()` e insere 3 personagens e 3 missoes se o banco estiver vazio. Para garantir seed limpo, apague `guilda.db` e execute a API novamente.

## Rodar a API
```powershell
cd ProjetoRPG
dotnet run --launch-profile https
```
- HTTPS: `https://localhost:7226` (Swagger em `https://localhost:7226/swagger`)
- HTTP: `http://localhost:5279` (Swagger em `http://localhost:5279/swagger`)
Se HTTPS reclamar do certificado de dev, rode `dotnet dev-certs https --trust` ou use HTTP.

Endpoints principais:
- `GET/POST/PUT/DELETE /api/personagens`
- `GET/POST/PUT/DELETE /api/missoes`
- `POST /api/missoes/{id}/personagens` body `{ personagemId, funcaoNoGrupo }`
- `DELETE /api/missoes/{id}/personagens/{personagemId}`

## Rodar o cliente WPF (GUI CRUD)
```powershell
cd GuildaClient
dotnet restore
dotnet run
```
- Defina a URL base na parte superior (padrao `https://localhost:7226`). Se der erro de certificado, clique em "Usar HTTP" para trocar para `http://localhost:5279` e depois "Recarregar tudo".
- Abas: Personagens e Missoes. Formulario lateral para criar/atualizar/excluir; botoes de Recarregar; secao de participantes na aba Missoes para atribuir/remover personagens.
- A barra de status exibe mensagens de sucesso/erro; erros detalhados aparecem em MessageBox.

## Testes rapidos (curl)
Criar personagem:
```powershell
curl -X POST https://localhost:7226/api/personagens -H "Content-Type: application/json" -d '{"nome":"Arthas","classe":1,"nivel":5,"vida":120,"mana":60,"moral":80}'
```
Criar missao:
```powershell
curl -X POST https://localhost:7226/api/missoes -H "Content-Type: application/json" -d '{"titulo":"Escoltar caravana","dificuldade":3,"recompensaOuro":150,"status":1}'
```
Atribuir personagem:
```powershell
curl -X POST https://localhost:7226/api/missoes/1/personagens -H "Content-Type: application/json" -d '{"personagemId":1,"funcaoNoGrupo":"Tank"}'
```
(Use http://localhost:5279 se optar por HTTP.)

## Troubleshooting
- 500 "no such table": delete `guilda.db`, rode `dotnet ef database update`, depois `dotnet run`. O seeder recria dados.
- Erro de permissao/Access is denied (OneDrive/antivirus): pause a sincronizacao e reexecute comandos ou mova o projeto para fora do OneDrive.
- WPF nao mostra dados: garanta API rodando; troque para HTTP; clique em "Recarregar tudo"; veja barra de status/MessageBox; confirme que `GET /api/personagens` responde no navegador.
- Certificado HTTPS: use `dotnet dev-certs https --trust` ou use HTTP na GUI.
- Migrations: use apenas `20251201032425_Inicial` (existe um arquivo duplicado desativado `20251201034500_Inicial.cs` para evitar conflito de nome).

## Arquivos de apoio
- `ProjetoRPG.http` tem chamadas prontas para personagens/missoes/atribuicoes.
- `WeatherForecast.cs` do template ainda existe; pode ser removido se desejar.
