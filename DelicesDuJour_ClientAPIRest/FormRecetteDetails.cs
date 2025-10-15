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

        //txtListeEtapesAfficherRecette.Font = new Font("Merienda", 14F, FontStyle.Regular);

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
                    // On efface tout le contenu du RichTextBox
                    rtxtListeEtapesAfficherRecette.Clear();

                    foreach (var etape in res.etapes)
                    {
                        // ----- TITRE -----
                        rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Bold);
                        rtxtListeEtapesAfficherRecette.SelectionColor = Color.FromArgb(42, 124, 204);
                        rtxtListeEtapesAfficherRecette.AppendText(etape.titre + Environment.NewLine);

                        // ----- TEXTE -----
                        rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Regular);
                        //rtxtListeEtapesAfficherRecette.SelectionColor = Color.Black;
                        rtxtListeEtapesAfficherRecette.AppendText(etape.texte + Environment.NewLine + Environment.NewLine);
                    }

                    // Revenir au style par défaut à la fin
                    rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Regular);
                    rtxtListeEtapesAfficherRecette.SelectionColor = Color.Black;
                }
                else
                {
                    rtxtListeEtapesAfficherRecette.Clear();
                    rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Bold);
                    rtxtListeEtapesAfficherRecette.SelectionColor = Color.Gray;
                    rtxtListeEtapesAfficherRecette.AppendText("Aucune étape trouvée.");
                }


                txtdifficulteAfficherRecette.Text = res.difficulte.ToString();

            }
        }

        finally
        {
            Cursor = Cursors.Default;

        }
    }

    private void FormRecetteDetails_FormClosing(object sender, FormClosingEventArgs e)
    {
        DialogResult res = MessageBox.Show("Confirmez-vous la fermeture de la fenêtre ?", "Fermeture", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        if (res == DialogResult.No)
        {
            e.Cancel = true;
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

