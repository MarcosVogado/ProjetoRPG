# Guilda RPG - API + WPF

Projeto ASP.NET Core 9 + EF Core 9 com tema de guilda de RPG. Possui API REST (controllers) e cliente WPF para CRUD de personagens e missões com relação N:N (participantes em missões).

## Requisitos atendidos
- CRUD via API para entidade principal (Personagem) e também Missão.
- Relacionamento entre entidades (Personagem, Missao, PersonagemMissao) e JOIN exposto nos endpoints de detalhes/listagem.
- Validações com DataAnnotations e retornos 400/404 em controllers.
- EF Core + SQLite com migrations.
- GUI: aplicativo WPF (`GuildaClient`) consumindo a API.
- Swagger/SwaggerUI em `/swagger` para testar via navegador.

## Estrutura
- `ProjetoRPG/` - API ASP.NET Core 9.
- `Data/RpgContext.cs` - DbContext e mapeamento da relação N:N.
- `Models/*.cs` - Entidades e enums (em português).
- `Controllers/PersonagensController.cs`, `Controllers/MissoesController.cs` - CRUD + atribuição/remocao de personagens em missões.
- `GuildaClient/` - Cliente WPF (net9.0-windows) para CRUD e atribuições.
- `ProjetoRPG.slnx` - lista os dois projetos.

## Pré-requisitos
- .NET SDK 9.0.x instalado (`dotnet --list-sdks` deve mostrar 9.0.x).
- Tool `dotnet-ef` alinhado com 9.x. Instale/atualize com:
  ```powershell
  dotnet tool uninstall --global dotnet-ef
  dotnet tool install --global dotnet-ef --version 9.0.1
  ```

## Preparar o banco (SQLite)
No diretório `ProjetoRPG`:
```powershell
cd ProjetoRPG
dotnet restore
# gerar migration inicial (uma vez)
dotnet ef migrations add Inicial
# aplicar no SQLite
dotnet ef database update
```
Obs.: se existir um `guilda.db` criado sem tabelas, apague antes (`Remove-Item guilda.db`).

## Rodar a API
```powershell
cd ProjetoRPG
dotnet run --launch-profile https
```
- HTTPS: `https://localhost:7226` (Swagger em `https://localhost:7226/swagger`).
- HTTP: `http://localhost:5279` (Swagger em `http://localhost:5279/swagger`).

Endpoints principais:
- `GET/POST/PUT/DELETE /api/personagens`
- `GET/POST/PUT/DELETE /api/missoes`
- `POST /api/missoes/{id}/personagens` (body: `{ personagemId, funcaoNoGrupo }`)
- `DELETE /api/missoes/{id}/personagens/{personagemId}`

## Rodar o cliente WPF
Em `GuildaClient`:
```powershell
cd GuildaClient
dotnet restore
dotnet run
```
- A tela traz duas abas: Personagens e Missoes.
- Configure a URL base (padrão `https://localhost:7226`) e clique em "Recarregar tudo".
- CRUD completo por botões e atribuição/remocao de personagens na missão selecionada.

## Testes rápidos (curl)
Criar personagem:
```powershell
curl -X POST https://localhost:7226/api/personagens -H "Content-Type: application/json" -d '{"nome":"Arthas","classe":1,"nivel":5,"vida":120,"mana":60,"moral":80}'
```
Criar missão:
```powershell
curl -X POST https://localhost:7226/api/missoes -H "Content-Type: application/json" -d '{"titulo":"Escoltar caravana","dificuldade":3,"recompensaOuro":150,"status":1}'
```
Atribuir personagem na missão:
```powershell
curl -X POST https://localhost:7226/api/missoes/1/personagens -H "Content-Type: application/json" -d '{"personagemId":1,"funcaoNoGrupo":"Tank"}'
```

## Dicas de validação/erros
- Data limite da missão deve ser futura ou vazia.
- Personagem e missão exigem campos obrigatórios ([Required], [Range], [StringLength]).
- Remoção/atribuição retornam 404 se não encontrados.

## Pendências conhecidas
- Arquivo `WeatherForecast.cs` do template permanece; pode ser apagado manualmente se desejar (OneDrive pode bloquear exclusão).
