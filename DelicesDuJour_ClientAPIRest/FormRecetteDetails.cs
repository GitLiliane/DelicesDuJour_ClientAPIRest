using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTOS;
using DelicesDuJour_ClientAPIRest.Services;
namespace DelicesDuJour_ClientAPIRest;

internal partial class FormRecetteDetails : Form
{
    private RecetteDTO _recette;
    private readonly RestClient _rest = RestClient.Instance;
    private readonly DeliceService _deliceService = DeliceService.Instance;
    private int idRecette;
    public FormRecetteDetails(int idRecetteDTO)
    {
        InitializeComponent();
        idRecette = idRecetteDTO;
    }

    private async void FormRecetteDetails_Load(object sender, EventArgs e)
    {
        InitializeBinding();

        try
        {
            var res = await _deliceService.GetRecetteByIdAsync(idRecette);

            if (res != null)
            {
                txtNomAfficherRecette.Text = res.nom;
                dtTempsPreparationAfficherRecette.Value = DateTime.Today.Add(res.temps_preparation);
                dtTempsCuissonAfficherRecette.Value = DateTime.Today.Add(res.temps_cuisson);
                txtdifficulteAfficherRecette.Text = res.difficulte.ToString();

                if (res.ingredients != null && res.ingredients.Any())
                {
                    rchListIngredientAfficherRecette.Text = string.Join(
                        Environment.NewLine,
                        res.ingredients.Select(i => $"{i.quantite}  {i.nom}")
                    );
                }
                else
                {
                    rchListIngredientAfficherRecette.Text = "Aucun ingrédient trouvé.";
                }
                if (res.etapes != null && res.etapes.Any())
                {
                    txtListeEtapesAfficherRecette.Text = string.Join(
                        Environment.NewLine,
                        res.etapes.Select(e => $"{e.titre} - {e.texte}")
                    );
                }
                else
                {
                    txtListeEtapesAfficherRecette.Text = "Aucune étape trouvée.";
                }
            }
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void InitializeBinding()
    {
        _recette = new();
        BSRecetteById.DataSource = _recette;
        txtNomAfficherRecette.DataBindings.Add("Text", BSRecetteById, "nom", false, DataSourceUpdateMode.Never);
        dtTempsPreparationAfficherRecette.DataBindings.Add("Text", BSRecetteById, "temps_preparation", false, DataSourceUpdateMode.Never);
        dtTempsCuissonAfficherRecette.DataBindings.Add("Text", BSRecetteById, "temps_cuisson", false, DataSourceUpdateMode.Never);
        txtdifficulteAfficherRecette.DataBindings.Add("Text", BSRecetteById, "difficulte", false, DataSourceUpdateMode.Never);

    }


}

