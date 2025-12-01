using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace GuildaClient;

public partial class MainWindow : Window
{
    private readonly HttpClient _http = new();
    private readonly ObservableCollection<Personagem> _personagens = new();
    private readonly ObservableCollection<Missao> _missoes = new();
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public MainWindow()
    {
        InitializeComponent();
        PersonagensGrid.ItemsSource = _personagens;
        MissoesGrid.ItemsSource = _missoes;
        Status("Pronto");
    }

    private string Api(string path) => $"{BaseUrlBox.Text.TrimEnd('/')}{path}";

    private void UsarHttp_Click(object sender, RoutedEventArgs e)
    {
        BaseUrlBox.Text = BaseUrlBox.Text.Replace("https://", "http://");
    }

    private async void RecarregarTudo_Click(object sender, RoutedEventArgs e)
    {
        await CarregarPersonagens();
        await CarregarMissoes();
    }

    private async void RecarregarPersonagens_Click(object sender, RoutedEventArgs e) => await CarregarPersonagens();
    private async void RecarregarMissoes_Click(object sender, RoutedEventArgs e) => await CarregarMissoes();

    private async Task CarregarPersonagens()
    {
        try
        {
            var resposta = await _http.GetAsync(Api("/api/personagens"));
            resposta.EnsureSuccessStatusCode();
            var dados = await resposta.Content.ReadFromJsonAsync<List<Personagem>>(_jsonOptions) ?? new();
            _personagens.Clear();
            foreach (var p in dados) _personagens.Add(p);
            Status($"{dados.Count} personagens carregados");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao carregar personagens", ex);
        }
    }

    private async Task CarregarMissoes()
    {
        try
        {
            var resposta = await _http.GetAsync(Api("/api/missoes"));
            resposta.EnsureSuccessStatusCode();
            var dados = await resposta.Content.ReadFromJsonAsync<List<Missao>>(_jsonOptions) ?? new();
            _missoes.Clear();
            foreach (var m in dados) _missoes.Add(m);
            AtualizarParticipantesLista(null);
            Status($"{dados.Count} missoes carregadas");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao carregar missoes", ex);
        }
    }

    private async void CriarPersonagem_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var novo = LerPersonagemDosInputs();
            var resposta = await _http.PostAsJsonAsync(Api("/api/personagens"), novo, _jsonOptions);
            resposta.EnsureSuccessStatusCode();
            await CarregarPersonagens();
            Status("Personagem criado");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao criar personagem", ex);
        }
    }

    private async void AtualizarPersonagem_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PersonagemIdBox.Text, out var id))
        {
            Status("Informe o Id para atualizar", true);
            return;
        }

        try
        {
            var p = LerPersonagemDosInputs();
            p.Id = id;
            var resposta = await _http.PutAsJsonAsync(Api($"/api/personagens/{id}"), p, _jsonOptions);
            if (!resposta.IsSuccessStatusCode)
            {
                var detalhe = await resposta.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha {resposta.StatusCode}: {detalhe}");
            }
            await CarregarPersonagens();
            Status("Personagem atualizado");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao atualizar personagem", ex);
        }
    }

    private async void ExcluirPersonagem_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(PersonagemIdBox.Text, out var id))
        {
            Status("Informe o Id para excluir", true);
            return;
        }

        try
        {
            var resposta = await _http.DeleteAsync(Api($"/api/personagens/{id}"));
            if (!resposta.IsSuccessStatusCode)
            {
                var detalhe = await resposta.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha {resposta.StatusCode}: {detalhe}");
            }
            await CarregarPersonagens();
            Status("Personagem excluido");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao excluir personagem", ex);
        }
    }

    private Personagem LerPersonagemDosInputs()
    {
        return new Personagem
        {
            Nome = NomeBox.Text,
            Classe = ClasseBox.Text,
            Nivel = ParseInt(NivelBox.Text),
            Vida = ParseInt(VidaBox.Text),
            Mana = ParseInt(ManaBox.Text),
            Moral = ParseInt(MoralBox.Text),
            PatenteGuilda = string.IsNullOrWhiteSpace(PatenteBox.Text) ? null : PatenteBox.Text
        };
    }

    private async void CriarMissao_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var nova = LerMissaoDosInputs();
            var resposta = await _http.PostAsJsonAsync(Api("/api/missoes"), nova, _jsonOptions);
            resposta.EnsureSuccessStatusCode();
            await CarregarMissoes();
            Status("Missao criada");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao criar missao", ex);
        }
    }

    private async void AtualizarMissao_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(MissaoIdBox.Text, out var id))
        {
            Status("Informe o Id para atualizar", true);
            return;
        }

        try
        {
            var missao = LerMissaoDosInputs();
            missao.Id = id;
            var resposta = await _http.PutAsJsonAsync(Api($"/api/missoes/{id}"), missao, _jsonOptions);
            if (!resposta.IsSuccessStatusCode)
            {
                var detalhe = await resposta.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha {resposta.StatusCode}: {detalhe}");
            }
            await CarregarMissoes();
            Status("Missao atualizada");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao atualizar missao", ex);
        }
    }

    private async void ExcluirMissao_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(MissaoIdBox.Text, out var id))
        {
            Status("Informe o Id para excluir", true);
            return;
        }

        try
        {
            var resposta = await _http.DeleteAsync(Api($"/api/missoes/{id}"));
            if (!resposta.IsSuccessStatusCode)
            {
                var detalhe = await resposta.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha {resposta.StatusCode}: {detalhe}");
            }
            await CarregarMissoes();
            Status("Missao excluida");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao excluir missao", ex);
        }
    }

    private Missao LerMissaoDosInputs()
    {
        DateOnly? data = null;
        if (DateOnly.TryParse(DataLimiteBox.Text, out var parsedData))
        {
            data = parsedData;
        }

        return new Missao
        {
            Titulo = TituloBox.Text,
            Descricao = string.IsNullOrWhiteSpace(DescricaoBox.Text) ? null : DescricaoBox.Text,
            Dificuldade = ParseInt(DificuldadeBox.Text),
            RecompensaOuro = ParseInt(OuroBox.Text),
            DataLimite = data,
            Status = ParseInt(StatusBox.Text)
        };
    }

    private async void AtribuirPersonagem_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(MissaoIdBox.Text, out var missaoId))
        {
            Status("Selecione/infome a missao para atribuir", true);
            return;
        }
        if (!int.TryParse(PersonagemIdParaMissaoBox.Text, out var personagemId))
        {
            Status("Informe o PersonagemId", true);
            return;
        }

        var payload = new { PersonagemId = personagemId, FuncaoNoGrupo = FuncaoBox.Text };
        try
        {
            var resposta = await _http.PostAsJsonAsync(Api($"/api/missoes/{missaoId}/personagens"), payload, _jsonOptions);
            if (!resposta.IsSuccessStatusCode)
            {
                var detalhe = await resposta.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha {resposta.StatusCode}: {detalhe}");
            }
            await CarregarMissoes();
            Status("Personagem atribuido");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao atribuir personagem", ex);
        }
    }

    private async void RemoverPersonagem_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(MissaoIdBox.Text, out var missaoId))
        {
            Status("Selecione/infome a missao para remover", true);
            return;
        }
        if (!int.TryParse(PersonagemIdParaMissaoBox.Text, out var personagemId))
        {
            Status("Informe o PersonagemId", true);
            return;
        }

        try
        {
            var resposta = await _http.DeleteAsync(Api($"/api/missoes/{missaoId}/personagens/{personagemId}"));
            if (!resposta.IsSuccessStatusCode)
            {
                var detalhe = await resposta.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha {resposta.StatusCode}: {detalhe}");
            }
            await CarregarMissoes();
            Status("Personagem removido");
        }
        catch (Exception ex)
        {
            MostrarErro("Erro ao remover personagem", ex);
        }
    }

    private void PersonagensGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (PersonagensGrid.SelectedItem is Personagem p)
        {
            PersonagemIdBox.Text = p.Id.ToString();
            NomeBox.Text = p.Nome;
            ClasseBox.Text = p.Classe;
            NivelBox.Text = p.Nivel.ToString();
            VidaBox.Text = p.Vida.ToString();
            ManaBox.Text = p.Mana.ToString();
            MoralBox.Text = p.Moral.ToString();
            PatenteBox.Text = p.PatenteGuilda;
        }
    }

    private void MissoesGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        AtualizarParticipantesLista(MissoesGrid.SelectedItem as Missao);
    }

    private void AtualizarParticipantesLista(Missao? missao)
    {
        ParticipantesList.Items.Clear();
        if (missao is null)
        {
            return;
        }

        MissaoIdBox.Text = missao.Id.ToString();
        TituloBox.Text = missao.Titulo;
        DescricaoBox.Text = missao.Descricao;
        DificuldadeBox.Text = missao.Dificuldade.ToString();
        OuroBox.Text = missao.RecompensaOuro.ToString();
        DataLimiteBox.Text = missao.DataLimite?.ToString("yyyy-MM-dd");
        StatusBox.Text = missao.Status.ToString();

        foreach (var p in missao.Participantes ?? new())
        {
            var nome = p.Personagem?.Nome ?? $"Personagem {p.PersonagemId}";
            ParticipantesList.Items.Add($"#{p.PersonagemId} - {nome} ({p.FuncaoNoGrupo})");
        }
    }

    private static int ParseInt(string? value)
    {
        return int.TryParse(value, out var n) ? n : 0;
    }

    private void Status(string mensagem, bool erro = false)
    {
        StatusText.Text = mensagem;
        StatusText.Foreground = erro ? System.Windows.Media.Brushes.OrangeRed : System.Windows.Media.Brushes.LightBlue;
    }

    private void MostrarErro(string titulo, Exception ex)
    {
        Status(titulo, true);
        MessageBox.Show(ex.Message, titulo, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

public class Personagem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Classe { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public int Vida { get; set; }
    public int Mana { get; set; }
    public int Moral { get; set; }
    public string? PatenteGuilda { get; set; }
}

public class Missao
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int Dificuldade { get; set; }
    public int RecompensaOuro { get; set; }
    public DateOnly? DataLimite { get; set; }
    public int Status { get; set; }
    public List<PersonagemMissao>? Participantes { get; set; }
}

public class PersonagemMissao
{
    public int PersonagemId { get; set; }
    public int MissaoId { get; set; }
    public string? FuncaoNoGrupo { get; set; }
    public Personagem? Personagem { get; set; }
}
