using DelicesDuJour_ClientAPIRest.Domain.DTO;
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

                if (res.temps_preparation != null)
                {
                    dtTempsPreparationAfficherRecette.Value = DateTime.Today.Add(res.temps_preparation);
                }
                else
                {
                    dtTempsPreparationAfficherRecette.Value = DateTime.Today; // ou une valeur par défaut
                }

                if (res.temps_cuisson != null)
                {
                    dtTempsCuissonAfficherRecette.Value = DateTime.Today.Add(res.temps_cuisson);
                }
                else
                {
                    dtTempsPreparationAfficherRecette.Value = DateTime.Today; // ou une valeur par défaut
                }

                txtdifficulteAfficherRecette.Text = res.difficulte.ToString();

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

