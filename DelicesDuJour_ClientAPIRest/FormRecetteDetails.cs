using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTOS;
using DelicesDuJour_ClientAPIRest.Services;

namespace DelicesDuJour_ClientAPIRest
{
    // Formulaire Windows Forms pour afficher les détails d'une recette spécifique.
    // Il permet de visualiser :
    // - le nom de la recette,
    // - les temps de préparation et de cuisson,
    // - la difficulté,
    // - la liste des ingrédients et étapes,
    // - la photo associée.
    internal partial class FormRecetteDetails : Form
    {
        private RecetteDTO _recette; // Objet recette lié aux bindings.
        private readonly RestClient _rest = RestClient.Instance; // Client REST singleton.
        private readonly DeliceService _deliceService = DeliceService.Instance; // Service métier singleton.
        private int idRecette; // ID de la recette affichée.

        // Constructeur du formulaire, prend l'ID de la recette à afficher.
        public FormRecetteDetails(int idRecetteDTO)
        {
            InitializeComponent(); // Initialise les composants WinForms.
            idRecette = idRecetteDTO;
        }

        RecetteDTO res = null; // Variable temporaire pour stocker les détails de la recette.

        // Permet de rafraîchir la recette affichée avec un nouvel ID.
        public void RefreshRecette(int newIdRecette)
        {
            idRecette = newIdRecette;
            RecetteDetails();
        }

        // Événement déclenché lorsque le formulaire se charge.
        private async void FormRecetteDetails_Load(object sender, EventArgs e)
        {
            InitializeBinding(); // Initialise les bindings de données.

            // Positionne la fenêtre par rapport à son parent si elle a un Owner.
            if (this.Owner != null)
            {
                var parent = this.Owner;
                this.Location = new Point(
                    parent.Location.X + parent.Width + 10, // Place à droite du parent avec un petit espace.
                    parent.Location.Y
                );
            }

            try
            {
                RecetteDetails(); // Charge les détails de la recette.
            }
            finally
            {
                Cursor = Cursors.Default; // Réinitialise le curseur.
            }
        }

        // Méthode principale pour charger et afficher les détails d'une recette.
        private async void RecetteDetails()
        {
            try
            {
                // Récupère la recette via le service métier.
                res = await _deliceService.GetRecetteByIdAsync(idRecette);

                if (res != null)
                {
                    // Affichage du nom, temps de préparation et cuisson, difficulté.
                    txtNomAfficherRecette.Text = res.nom;
                    dtTempsPreparationAfficherRecette.Value = DateTime.Today.Add(res.temps_preparation);
                    dtTempsCuissonAfficherRecette.Value = DateTime.Today.Add(res.temps_cuisson);
                    txtdifficulteAfficherRecette.Text = res.difficulte.ToString();

                    // Affichage des ingrédients.
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

                    // Affichage des étapes.
                    if (res.etapes != null && res.etapes.Any())
                    {
                        rtxtListeEtapesAfficherRecette.Clear();

                        foreach (var etape in res.etapes)
                        {
                            // ----- TITRE en gras et couleur bleu -----
                            rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Bold);
                            rtxtListeEtapesAfficherRecette.SelectionColor = Color.FromArgb(42, 124, 204);
                            rtxtListeEtapesAfficherRecette.AppendText(etape.titre + Environment.NewLine);

                            // ----- TEXTE en police normale -----
                            rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Regular);
                            rtxtListeEtapesAfficherRecette.AppendText(etape.texte + Environment.NewLine + Environment.NewLine);
                        }

                        // Revenir au style par défaut à la fin
                        rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Regular);
                        rtxtListeEtapesAfficherRecette.SelectionColor = Color.Black;
                    }
                    else
                    {
                        // Si aucune étape, message par défaut.
                        rtxtListeEtapesAfficherRecette.Clear();
                        rtxtListeEtapesAfficherRecette.SelectionFont = new Font(rtxtListeEtapesAfficherRecette.Font, FontStyle.Bold);
                        rtxtListeEtapesAfficherRecette.SelectionColor = Color.Gray;
                        rtxtListeEtapesAfficherRecette.AppendText("Aucune étape trouvée.");
                    }

                    // Préparation de la PictureBox pour l'affichage de l'image.
                    pbxImageRecette.SizeMode = PictureBoxSizeMode.Zoom;

                    // Gestion du chemin de l'image (relatif ou absolu).
                    string baseUrl = "http://localhost:5289"; // URL de base à adapter.
                    string imageUrl = !string.IsNullOrEmpty(res.photo)
                        ? (res.photo.StartsWith("http") ? res.photo : $"{baseUrl}{res.photo}")
                        : null;

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        try
                        {
                            pbxImageRecette.LoadAsync(imageUrl); // Charge l'image de manière asynchrone.
                        }
                        catch
                        {
                            // En cas d'erreur de chargement, utilise l'image d'erreur par défaut.
                            pbxImageRecette.Image = pbxImageRecette.ErrorImage;
                        }
                    }
                    else
                    {
                        // Si pas d'image disponible, utilise l'image initiale par défaut.
                        pbxImageRecette.Image = pbxImageRecette.InitialImage;
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default; // Réinitialise le curseur.
            }
        }

        // Événement déclenché à la fermeture du formulaire.
        private void FormRecetteDetails_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show(
                "Confirmez-vous la fermeture de la fenêtre ?",
                "Fermeture",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            if (res == DialogResult.No)
            {
                e.Cancel = true; // Annule la fermeture si l'utilisateur choisit "Non".
            }
        }

        // Initialise les bindings entre les contrôles WinForms et l'objet RecetteDTO.
        private void InitializeBinding()
        {
            _recette = new(); // Crée un nouvel objet RecetteDTO vide.
            BSRecetteById.DataSource = _recette;

            txtNomAfficherRecette.DataBindings.Add("Text", BSRecetteById, "nom", false, DataSourceUpdateMode.Never);
            dtTempsPreparationAfficherRecette.DataBindings.Add("Text", BSRecetteById, "temps_preparation", false, DataSourceUpdateMode.Never);
            dtTempsCuissonAfficherRecette.DataBindings.Add("Text", BSRecetteById, "temps_cuisson", false, DataSourceUpdateMode.Never);
            txtdifficulteAfficherRecette.DataBindings.Add("Text", BSRecetteById, "difficulte", false, DataSourceUpdateMode.Never);
        }
    }
}
