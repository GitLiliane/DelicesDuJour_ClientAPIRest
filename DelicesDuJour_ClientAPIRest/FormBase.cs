using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using DelicesDuJour_ClientAPIRest.Domain.DTOS;
using DelicesDuJour_ClientAPIRest.Services;
using ReaLTaiizor.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabPage = System.Windows.Forms.TabPage;

namespace DelicesDuJour_ClientAPIRest
{
    public partial class FormBase : Form
    {
        // Mode d'affichage selon qu'on filtre par catégorie ou non
        bool modeCategorie = false;

        // Liste des onglets pour gérer leur visibilité
        List<TabPage> _tabPages = [];

        // Rôles récupérés après connexion
        IEnumerable<string> _roles = [];

        // BindingLists pour lier les données aux DataGridViews et aux ListBoxes
        BindingList<RecetteDTO> _recettes;
        BindingList<CategorieDTO> _categories;
        BindingList<RecetteDTO> _recettesByCategorie;
        BindingList<CategorieDTO> _categoriesByRecette;
        BindingList<RecetteCategorieRelationshipDTO> _recettesCategoriesRelations;
        BindingList<IngredientDTO> _ingredients = new();
        BindingList<EtapeDTO> _etapes = new();
        BindingList<RecetteDTO> _gestionRecettes;

        // Service unique pour appeler l’API
        private readonly DeliceService _deliceService = DeliceService.Instance;

        // Formulaire pour afficher les détails d’une recette
        FormRecetteDetails? detailform;

        public FormBase()
        {
            InitializeComponent();

            // Active les boutons natifs de la fenêtre
            metroControlBox1.MinimizeBox = true;
            metroControlBox1.MaximizeBox = true;
        }

        // Méthode appelée au chargement du formulaire
        private async void FormBase_Load(object sender, EventArgs e)
        {
            InitializeBinding(); // Initialise le binding entre les listes et les DataGridViews

            // Récupère la liste des onglets existants
            _tabPages = tabControl.TabPages.Cast<TabPage>().ToList();

            // Charge les paramètres sauvegardés
            txtHttp.Text = Properties.Settings.Default.Http;
            txtIdentifiant.Text = Properties.Settings.Default.Identifiant;

            // Active ou désactive les onglets selon les rôles
            TabPagesAuthorizations();

            // Personnalisation graphique des onglets
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += tabControl_DrawItem;
            tabControl.MouseHover += tabControl_MouseHover;
            tabControl.MouseLeave += tabControl_MouseLeave;

            // Définit l'onglet par défaut sur Login
            tabControl.SelectedTab = tabLogin;
            await Task.Delay(50);
            txtPassword.Focus();
            AcceptButton = btLogin;

            // Gestion de la sélection dans le DataGridView recettes
            dgvRecettes.SelectionChanged += DgvRecettes_SelectionChanged;
        }
            

        // Confirme la fermeture de l’application
        private void FormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirmez-vous la fermeture de l'application ?", "Fermeture", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (res == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        // Sauvegarde des paramètres à la fermeture
        private void FormBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Http = txtHttp.Text;
            Properties.Settings.Default.Identifiant = txtIdentifiant.Text;
            Properties.Settings.Default.Save();

            btLogOut.PerformLayout();
        }

        // Gestion de la sélection d’un onglet avant affichage
        private async void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tabLogin)
            {
                AcceptButton = btLogin;
            }
            else if (e.TabPage == tabRecettes)
            {
                await ActualiserRecettes();
                AcceptButton = btTtesRecettes;
            }
            else if (e.TabPage == tabCategories)
            {
                await ActualiserCategories();
                AcceptButton = btActualiserCategorie;
            }
            else if (e.TabPage == tabRecetteCategorie)
            {
                await ActualiserRecettes();
                await ActualiserRecettesCategoriesRelations();
            }
            else if (e.TabPage == tabGestionRecette)
            {
                await ActualiserRecettes();
                await ActualiserCategories();
                clbCategories.DataSource = _categories;
                clbCategories.DisplayMember = "nom";
                clbCategories.Refresh();
                tpImage.SetToolTip(btAjouterImage, "Cliquez pour ajouter une image.");
                tltAjouterIngredient.SetToolTip(btAjouterIngredient, "Cliquez pour ajouter un ingrédient.");
                tltSupprimerIngredient.SetToolTip(btSupprimerIngredient, "Cliquez pour supprimer un ingrédient.");
                tltAjouterEtape.SetToolTip(btEtapeAjouter, "Cliquez pour ajouter une étape.");
                tltSupprimerEtape.SetToolTip(btEtapeSupprimer, "Cliquez pour supprimer une étape.");
            }
        }

        // Change la couleur des onglets selon qu’ils soient sélectionnés ou non
        private async void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (TabPage page in tabControl.TabPages)
            {
                // Couleur par défaut pour les non-sélectionnées
                page.BackColor = Color.FromArgb(251, 249, 233);
                page.ForeColor = Color.FromArgb(33, 34, 69);
            }

            // Couleur spéciale pour l’onglet actif
            TabPage selectedPage = tabControl.SelectedTab;
            selectedPage.BackColor = Color.FromArgb(251, 249, 233);
            selectedPage.ForeColor = Color.FromArgb(33, 34, 69);
        }

        // Indice de l’onglet survolé par la souris
        private int _hoveredIndex = -1;

        // Personnalisation du dessin des onglets
        private async void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage page = tabControl.TabPages[e.Index];
            Rectangle rect = e.Bounds;

            bool isSelected = (e.Index == tabControl.SelectedIndex);
            bool isHovered = (e.Index == _hoveredIndex);

            Color selectedColor = Color.FromArgb(53, 155, 255);
            Color hoverColor = Color.FromArgb(189, 183, 107);
            Color normalColor = Color.FromArgb(75, 73, 67);

            Color textColor = Color.White;

            Color backColor = isSelected
                ? selectedColor
                : isHovered
                    ? hoverColor
                    : normalColor;

            // Fond de l’onglet
            using (SolidBrush brush = new(backColor))
                e.Graphics.FillRectangle(brush, rect);

            // Texte centré
            TextRenderer.DrawText(
                e.Graphics,
                page.Text,
                new Font("Segoe UI", 12, FontStyle.Bold),
                rect,
                textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            );
        }

        // Gestion du survol de la souris sur les onglets
        private void tabControl_MouseHover(object sender, EventArgs e)
        {
            Point mousePos = tabControl.PointToClient(Cursor.Position);

            for (int i = 0; i < tabControl.TabCount; i++)
            {
                if (tabControl.GetTabRect(i).Contains(mousePos))
                {
                    if (_hoveredIndex != i)
                    {
                        _hoveredIndex = i;
                        tabControl.Invalidate(); // Redessine les onglets
                    }
                    return;
                }
            }

            if (_hoveredIndex != -1)
            {
                _hoveredIndex = -1;
                tabControl.Invalidate();
            }
        }

        // Remise à zéro du survol lorsque la souris quitte le tabControl
        private async void tabControl_MouseLeave(object sender, EventArgs e)
        {
            _hoveredIndex = -1;
            tabControl.Invalidate();
        }

        #region Login
        // Gestion de la connexion
        private async void btLogin_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                LoginDTO loginDTO = new()
                {
                    Username = txtIdentifiant.Text,
                    Password = txtPassword.Text
                };

                bool res = await _deliceService.Login(txtHttp.Text, loginDTO);

                if (res)
                {
                    txtPassword.Text = string.Empty;

                    _roles = _deliceService.GetRolesFromJwt();
                    lblRoles.Text = string.Join(", ", _roles);

                    TabPagesAuthorizations(); // Affiche les onglets selon les rôles

                    tabLogin.Parent = null; // Masque l’onglet login
                    tabControl.SelectedTab = tabRecettes;
                }
                else
                {
                    MessageBox.Show("Problème de connexion");
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            ActualiserRecettes();
        }

        // Gestion de la déconnexion
        private async void btLogOut_Click(object sender, EventArgs e)
        {
            _deliceService.Logout();

            // Réinitialisation de toutes les listes et bindings
            _roles = [];
            _recettes.Clear();
            _categories.Clear();
            _recettesByCategorie.Clear();
            _categoriesByRecette.Clear();
            _recettesCategoriesRelations.Clear();
            _ingredients.Clear();
            _etapes.Clear();
            _gestionRecettes.Clear();

            lblRoles.Text = string.Empty;
            txtPassword.Text = string.Empty;

            TabPagesAuthorizations(); // Actualise les onglets
        }
        #endregion Fin Login


        #region recettes

        private async void btTtesRecettes_Click(object sender, EventArgs e)
        {
            // Passage en mode "toutes les recettes" : on ne filtre pas par catégorie
            modeCategorie = false;

            // Actualise la liste complète des recettes depuis le service
            ActualiserRecettes();

            // Met à jour le BindingSource pour afficher les recettes dans le DataGridView
            ChangeBSRecettes();
        }

        private async void btEntree_Click(object sender, EventArgs e)
        {
            try
            {
                // Passage en mode "filtrage par catégorie"
                modeCategorie = true;

                // Change le curseur pour indiquer un traitement en cours
                Cursor = Cursors.WaitCursor;

                // Récupère les catégories depuis le service si elles ne sont pas déjà chargées
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Entrée" dans la liste _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Entrée", StringComparison.OrdinalIgnoreCase));

                int idEntree = entreeCategorie.Id; // récupère l'ID de la catégorie

                // Étape 2 : récupère les recettes associées à cette catégorie via l'API
                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);

                if (recettes.Any())
                {
                    // Étape 3 : vide la liste existante et ajoute les recettes récupérées
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }

                    // Met à jour le DataGridView pour afficher uniquement ces recettes
                    ChangeDgvByCategorie();
                }
                else
                {
                    // Affiche un message si aucune recette n'est trouvée pour la catégorie
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            finally
            {
                // Réinitialise le curseur quelle que soit l'issue
                Cursor = Cursors.Default;
            }
        }

        private async void btPlat_Click(object sender, EventArgs e)
        {
            try
            {
                // Passage en mode filtrage par catégorie
                modeCategorie = true;

                // Curseur d'attente
                Cursor = Cursors.WaitCursor;

                // Récupération des catégories
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Plat" dans _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Plat", StringComparison.OrdinalIgnoreCase));

                int idEntree = entreeCategorie.Id; // récupération de l'ID de la catégorie

                // Étape 2 : récupérer les recettes correspondant à cette catégorie via le service
                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);
                if (recettes.Any())
                {
                    // Étape 3 : vider la liste existante et ajouter les recettes récupérées
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }

                    // Mettre à jour le DataGridView pour afficher ces recettes
                    ChangeDgvByCategorie();
                }
                else
                {
                    // Affichage d'un message si aucune recette n'est trouvée
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Réinitialisation du curseur
                Cursor = Cursors.Default;
            }
        }
        private async void btDessert_Click(object sender, EventArgs e)
        {
            try
            {
                // Passage en mode filtrage par catégorie
                modeCategorie = true;

                // Change le curseur pour indiquer que le traitement est en cours
                Cursor = Cursors.WaitCursor;

                // Récupère toutes les catégories depuis le service si nécessaire
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Dessert" dans la liste _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Dessert", StringComparison.OrdinalIgnoreCase));

                // Étape 2 : récupère l'ID de la catégorie pour interroger les recettes
                int idEntree = entreeCategorie.Id;

                // Étape 3 : récupérer les recettes correspondant à cette catégorie via le service
                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);
                if (recettes.Any())
                {
                    // Vider la liste existante et ajouter les recettes récupérées
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }

                    // Mettre à jour le DataGridView pour n'afficher que ces recettes
                    ChangeDgvByCategorie();
                }
                else
                {
                    // Affiche un message si aucune recette n’est trouvée
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Toujours réinitialiser le curseur quelle que soit l'issue
                Cursor = Cursors.Default;
            }
        }

        private async void btSoupe_Click(object sender, EventArgs e)
        {
            try
            {
                // Passage en mode filtrage par catégorie
                modeCategorie = true;

                // Curseur d'attente pour l'utilisateur
                Cursor = Cursors.WaitCursor;

                // Récupération des catégories
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Soupe" dans _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Soupe", StringComparison.OrdinalIgnoreCase));

                // Étape 2 : récupérer l'ID pour interroger les recettes correspondantes
                int idEntree = entreeCategorie.Id;

                // Étape 3 : récupérer les recettes de la catégorie "Soupe" via le service
                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);

                if (recettes.Any())
                {
                    // Vider la liste existante et ajouter les nouvelles recettes
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }

                    // Mise à jour du DataGridView pour afficher ces recettes
                    ChangeDgvByCategorie();
                }
                else
                {
                    // Message si aucune recette n'est trouvée
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            finally
            {
                // Réinitialisation du curseur
                Cursor = Cursors.Default;
            }
        }

        private async void btDetailsRecette_Click(object sender, EventArgs e)
        {
            // Récupère la recette sélectionnée selon le mode actuel
            // Si modeCategorie est true, on prend la recette dans la liste filtrée par catégorie
            var recette = ((modeCategorie) ? BSRecettesByCategorie.Current : BSRecettes.Current) as RecetteDTO;

            // Si aucune recette n'est sélectionnée, on quitte la méthode
            if (recette is null)
            {
                //MessageBox.Show("Veuillez sélectionner une recette.",
                //                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Si le formulaire de détails n'existe pas ou a été fermé, on le crée
            if (detailform is null || detailform.IsDisposed)
            {
                detailform = new FormRecetteDetails(recette.Id)
                {
                    Owner = this // Définit le formulaire parent pour la gestion des fenêtres
                };
            }
            else
                // Si le formulaire existe déjà, on met à jour les informations de la recette
                detailform.RefreshRecette(recette.Id);

            // Affiche le formulaire de détails
            detailform.Show();
        }

        private async Task ActualiserRecettes()
        {
            // Sauvegarde de la recette actuellement sélectionnée pour pouvoir revenir dessus après mise à jour
            RecetteDTO current = BSRecettes.Current as RecetteDTO;

            // Récupération de toutes les recettes depuis le service
            var result = await _deliceService.GetRecettesAsync();

            if (result.Any())
            {
                // Vide la liste actuelle et ajoute toutes les recettes récupérées
                _recettes.Clear();
                foreach (RecetteDTO r in result)
                    _recettes.Add(r);
            }
            else
            {
                // Affiche un message si aucune recette n’a été trouvée
                MessageBox.Show("Aucune recette trouvée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Repositionne la sélection sur la recette qui était active avant la mise à jour
            if (current is not null)
                BSRecettes.Position = _recettes.IndexOf(_recettes.Where(r => r.Id == current.Id).FirstOrDefault());
        }

        // Méthode déclenchée lors d’un changement de sélection dans le DataGridView Recettes
        private void DgvRecettes_SelectionChanged(object? sender, EventArgs e)
        {
            if (detailform is not null && tabControl.SelectedTab == tabRecettes)
            {
                btDetailsRecette_Click(sender, e); // Ouvre ou rafraîchit le détail de la recette
            }
        }
        #endregion Fin Gestion recettes

        #region Catégories

        // Rafraîchit la liste des catégories depuis le service
        private async void btActualiserCategorie_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor; // Affiche le curseur "attente"
                await ActualiserCategories(); // Appel à la méthode qui récupère les catégories
            }
            finally
            {
                Cursor = Cursors.Default; // Remet le curseur normal
            }
        }

        // Ajoute une nouvelle catégorie
        private async void btAjouterCategorie_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Crée un DTO avec le nom de la nouvelle catégorie
                CreateCategorieDTO createDTO = new()
                {
                    nom = txtNomCategories.Text,
                };

                // Appel du service pour ajouter la catégorie
                var res = await _deliceService.AddCategorieAsync(createDTO);

                if (res != null)
                {
                    MessageBox.Show("La nouvelle catégorie a bien été ajoutée", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Rafraîchit la liste et positionne la sélection sur la nouvelle catégorie
                    await ActualiserCategories();
                    BSCategories.Position = _categories.IndexOf(_categories.FirstOrDefault(b => b.Id == res.Id));
                }
                else
                {
                    MessageBox.Show("Une erreur s'est produite. Impossible de créer la catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Modifie la catégorie sélectionnée
        private async void btModifierCategorie_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Récupère la catégorie actuellement sélectionnée
                CategorieDTO current = BSCategories.Current as CategorieDTO;

                if (current is not null)
                {
                    // Crée un DTO avec les nouvelles informations
                    UpdateCategorieDTO updateDTO = new()
                    {
                        nom = txtNomCategories.Text
                    };

                    // Appel du service pour mettre à jour la catégorie
                    var res = await _deliceService.UpdateCategorieAsync(updateDTO, current.Id);

                    if (res != null)
                    {
                        MessageBox.Show("La catégorie a bien été modifiée", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Rafraîchit la liste et positionne la sélection sur la catégorie modifiée
                        await ActualiserCategories();
                        BSCategories.Position = _categories.IndexOf(_categories.FirstOrDefault(b => b.Id == res.Id));
                    }
                    else
                    {
                        MessageBox.Show("Une erreur s'est produite. Impossible de modifier la catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Supprime la catégorie sélectionnée
        private async void btSupprimerCategorie_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                CategorieDTO current = BSCategories.Current as CategorieDTO;

                if (current is not null)
                {
                    // Demande de confirmation avant suppression
                    if ((MessageBox.Show($"Confirmez-vous la suppression de la catégorie {current.nom} ?",
                        "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                    {
                        await _deliceService.DeleteCategorieAsync(current.Id);
                        MessageBox.Show("La catégorie a bien été supprimée", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // Rafraîchit la liste après suppression
                    await ActualiserCategories();
                }

            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Méthode pour récupérer et remplir la liste des catégories depuis le service
        private async Task ActualiserCategories()
        {
            // Sauvegarde de la catégorie actuellement sélectionnée
            CategorieDTO current = BSCategories.Current as CategorieDTO;

            // Récupération de toutes les catégories via le service
            var res = await _deliceService.GetCategoriesAsync();

            if (res.Any())
            {
                // Vide la liste et ajoute les catégories récupérées
                _categories.Clear();
                foreach (CategorieDTO c in res)
                    _categories.Add(c);
            }
            else
            {
                MessageBox.Show("Aucune catégorie trouvée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Repositionne la sélection sur la catégorie qui était active avant la mise à jour
            if (current is not null)
                BSCategories.Position = _categories.IndexOf(_categories.Where(b => b.Id == current.Id).FirstOrDefault());
        }

        #endregion Fin Categories



        #region Relation Recettes Catégories

        // Récupère les recettes d'une catégorie à partir de l'ID ou du nom
        private async void btGetRecByCat_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // S'assurer que les catégories sont à jour
                await ActualiserCategories();

                // Essayer de récupérer la catégorie à partir de l'ID ou du nom
                int recetteId = 0;
                var idCatOk = int.TryParse(txtIdCategorie.Text, out int id);
                var nomCat = txtNomCategorie.Text;

                var catById = _categories.FirstOrDefault(c => c.Id == id);
                var catByNom = _categories.FirstOrDefault(c => c.nom == nomCat);

                if (catById is not null)
                    recetteId = catById.Id;
                else if (catByNom is not null)
                    recetteId = catByNom.Id;

                // Récupération des recettes via le service
                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(recetteId);

                if (recettes.Any())
                {
                    _recettesByCategorie.Clear();
                    foreach (var r in recettes)
                        _recettesByCategorie.Add(r);

                    ChangeDgvRByC(); // Mise à jour du DataGridView correspondant
                }
                else
                {
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                await ActualiserCategories();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Effets visuels sur le bouton
        private void btGetRecByCat_MouseMove(object sender, MouseEventArgs e) => btGetRecByCat.ForeColor = Color.FromArgb(182, 204, 254);
        private void btGetRecByCat_MouseLeave(object sender, EventArgs e) => btGetRecByCat.ForeColor = Color.WhiteSmoke;

        // Récupère les catégories d'une recette à partir de l'ID ou du nom
        private async void btGetCatByRec_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                await ActualiserRecettes();

                int categorieId = 0;
                var idRecOk = int.TryParse(txtIdrecette.Text, out int id);
                var nomRec = txtNomRecette.Text;

                var recById = _recettes.FirstOrDefault(r => r.Id == id);
                var recByNom = _recettes.FirstOrDefault(r => r.nom == nomRec);

                if (recById is not null)
                    categorieId = recById.Id;
                else if (recByNom is not null)
                    categorieId = recByNom.Id;

                var categories = await _deliceService.GetCategoriesByIdRecette(categorieId);

                if (categories.Any())
                {
                    _categoriesByRecette.Clear();
                    foreach (var c in categories)
                        _categoriesByRecette.Add(c);

                    ChangeDgvByR(); // Mise à jour du DataGridView correspondant
                }
                else
                {
                    MessageBox.Show("Aucune catégorie ne correspond à cette recette.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Effets visuels sur le bouton
        private void btGetCatByRec_MouseHover(object sender, EventArgs e) => btGetCatByRec.ForeColor = Color.FromArgb(182, 204, 254);
        private void btGetCatByRec_MouseLeave(object sender, EventArgs e) => btGetCatByRec.ForeColor = Color.WhiteSmoke;

        // Ajoute une relation entre une recette et une catégorie
        private async void btAjouterRelationRecCat_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                await ActualiserRecettesCategoriesRelations();

                int idRecette = int.Parse(txtRelationIdRecette.Text);
                int idCategorie = int.Parse(txtRelationIdCategorie.Text);

                await _deliceService.AddRelationRecetteCategorieAsync(idCategorie, idRecette);

                MessageBox.Show("La création de la relation a bien été faite.", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await ActualiserRecettesCategoriesRelations();

                BSRecettesCategoriesRelations.Position = _recettesCategoriesRelations
                    .IndexOf(_recettesCategoriesRelations.FirstOrDefault(r => r.idRecette == idRecette));
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Supprime une relation entre une recette et une catégorie
        private async void btSupprimerRelationRecCat_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                int idRecette = int.Parse(txtRelationIdRecette.Text);
                int idCategorie = int.Parse(txtRelationIdCategorie.Text);

                if (MessageBox.Show("Confirmez-vous la suppression de la relation ?",
                    "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    await _deliceService.DeleteRelationRecetteCategorieAsync(idCategorie, idRecette);
                    MessageBox.Show("La relation a bien été supprimée", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await ActualiserRecettesCategoriesRelations();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Rafraîchit la liste des relations recettes/catégories
        private async Task ActualiserRecettesCategoriesRelations()
        {
            var current = BSRecettesCategoriesRelations.Current as RecetteCategorieRelationshipDTO;

            var res = await _deliceService.GetRecetteCategorieRelationshipsAsync();

            _recettesCategoriesRelations.Clear();
            foreach (var r in res)
                _recettesCategoriesRelations.Add(r);

            if (current is not null)
                BSRecettesCategoriesRelations.Position = _recettesCategoriesRelations
                    .IndexOf(_recettesCategoriesRelations.FirstOrDefault(r => r.idRecette == current.idRecette));
        }

        #endregion Fin Relation Recettes Catégories


        #region Gestion des Recettes

        // Lorsqu'on clique sur une cellule du DataGridView des recettes
        private async void dgvGestionRecette_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                // Récupère la recette sélectionnée
                RecetteDTO current = BSRecettes.Current as RecetteDTO;
                if (current is null) return;

                var currentRecette = await _deliceService.GetRecetteByIdAsync(current.Id);
                if (currentRecette is null) return;

                // Remplir les champs du formulaire
                txtIdGestionrecette.Text = currentRecette.Id.ToString();
                txtTitreRecette.Text = currentRecette.nom;
                dtpTempsPreparation.Value = DateTime.Today.Add(currentRecette.temps_preparation);
                dtpTempsCuisson.Value = DateTime.Today.Add(currentRecette.temps_cuisson);
                listBoxDifficulte.SelectedItem = currentRecette.difficulte.ToString();

                // Décocher toutes les catégories
                for (int i = 0; i < clbCategories.Items.Count; i++)
                {
                    clbCategories.SetItemChecked(i, false);
                }

                // Cocher les catégories de la recette
                foreach (var categorie in currentRecette.categories)
                {
                    for (int i = 0; i < clbCategories.Items.Count; i++)
                    {
                        if (clbCategories.Items[i] is CategorieDTO item && item.Id == categorie.Id)
                        {
                            clbCategories.SetItemChecked(i, true);
                        }
                    }
                }

                // Actualiser les listes liées d'ingrédients et d'étapes
                _ingredients.Clear();
                foreach (var ingredient in currentRecette.ingredients)
                    _ingredients.Add(ingredient);

                _etapes.Clear();
                foreach (var etape in currentRecette.etapes)
                    _etapes.Add(etape);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Ajouter un ingrédient à la recette en cours
        private async void btAjouterIngredient_Click(object sender, EventArgs e)
        {
            string nom = txtNomIngredientAjouter.Text.Trim();
            string quantite = txtQuantiteIngredientAjouter.Text.Trim();

            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(quantite))
            {
                MessageBox.Show("Veuillez renseigner à la fois le nom et la quantité.",
                                "Ajout d'ingrédient", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            IngredientDTO ingredient = new()
            {
                nom = nom,
                quantite = quantite
            };

            // Ajout à la liste liée au DataGridView
            _ingredients.Add(ingredient);

            // Ajustement automatique des colonnes
            dgvIngredientAjouter.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // Vider les champs après ajout
            txtNomIngredientAjouter.Clear();
            txtQuantiteIngredientAjouter.Clear();
        }


        // Supprimer l'ingrédient sélectionné dans le DataGridView
        private async void btSupprimerIngredient_Click(object sender, EventArgs e)
        {
            if (dgvIngredientAjouter.CurrentRow is null) return;

            var current = dgvIngredientAjouter.CurrentRow.DataBoundItem as IngredientDTO;
            if (current is null) return;

            // Confirmation de la suppression
            if (MessageBox.Show(
                    "Confirmez-vous la suppression de l'ingrédient ?",
                    "Supprimer",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                _ingredients.Remove(current);
                MessageBox.Show("L'ingrédient sélectionné a bien été supprimé.", "Supprimer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Ajouter une étape à la recette en cours
        private async void btEtapeAjouter_Click(object sender, EventArgs e)
        {
            string numeroText = txtNumeroEtapeAjouter.Text.Trim();
            string titre = txtTitreEtapeAjouter.Text.Trim();
            string texte = txtTexteEtapeAjouter.Text.Trim();

            if (string.IsNullOrEmpty(numeroText) || string.IsNullOrEmpty(titre) || string.IsNullOrEmpty(texte))
            {
                MessageBox.Show(
                    "Veuillez renseigner à la fois le numéro, le titre et le texte de l'étape.",
                    "Ajout d'étape",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(numeroText, out int numero))
            {
                MessageBox.Show("Le numéro de l'étape doit être un entier.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EtapeDTO etape = new()
            {
                numero = numero,
                titre = titre,
                texte = texte
            };

            // Ajout à la liste liée au DataGridView
            _etapes.Add(etape);

            // Ajustement automatique des colonnes
            dgvEtapeAjouter.Columns["Titre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // Vider les champs après ajout
            txtNumeroEtapeAjouter.Clear();
            txtTitreEtapeAjouter.Clear();
            txtTexteEtapeAjouter.Clear();
        }


        // Supprimer l'étape sélectionnée dans le DataGridView
        private async void btEtapeSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvEtapeAjouter.CurrentRow is null) return;

            var current = dgvEtapeAjouter.CurrentRow.DataBoundItem as EtapeDTO;
            if (current is null) return;

            // Confirmation de la suppression
            if (MessageBox.Show(
                    "Confirmez-vous la suppression de l'étape ?",
                    "Supprimer",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                _etapes.Remove(current);
                MessageBox.Show("L'étape sélectionnée a bien été supprimée.", "Supprimer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Ajouter ou sélectionner une image pour la recette
        private void btAjouterImage_Click(object sender, EventArgs e)
        {
            // Réinitialisation du FileName
            openFileDialogImageRecette.FileName = string.Empty;

            // Restaurer le dernier dossier utilisé
            openFileDialogImageRecette.RestoreDirectory = true;

            // Filtre pour les types d'image autorisés
            openFileDialogImageRecette.Filter = "Images|*.jpg;*.jpeg;*.png";
            openFileDialogImageRecette.Title = "Choisir une image pour la recette";

            // Réinitialiser le champ texte avant la sélection
            txtImage.Text = string.Empty;

            // Afficher la boîte de dialogue de sélection de fichier
            DialogResult resultat = openFileDialogImageRecette.ShowDialog();

            // Vérifier si l'utilisateur a cliqué sur 'Ouvrir'
            if (resultat == DialogResult.OK)
            {
                txtImage.Text = openFileDialogImageRecette.FileName;
            }
        }

        // Ajouter une nouvelle recette
        private async void btAjouterRecette_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Vérification des champs requis
                if (string.IsNullOrWhiteSpace(txtTitreRecette.Text) ||
                    listBoxDifficulte.SelectedItem == null ||
                    _ingredients.Count == 0 ||
                    _etapes.Count == 0)
                {
                    MessageBox.Show(
                        "Veuillez renseigner tous les champs requis : titre, difficulté, ingrédients et étapes.",
                        "Ajout de recette",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                // Récupération des catégories cochées
                List<CategorieDTO> categoriesSelectionnees = clbCategories.CheckedItems
                    .OfType<CategorieDTO>()
                    .ToList();

                int difficulte = int.Parse(listBoxDifficulte.SelectedItem.ToString());

                // Création du DTO
                CreateRecetteDTO createRecetteDTO = new()
                {
                    nom = txtTitreRecette.Text,
                    temps_preparation = dtpTempsPreparation.Value.TimeOfDay,
                    temps_cuisson = dtpTempsCuisson.Value.TimeOfDay,
                    difficulte = difficulte,
                    categories = categoriesSelectionnees,
                    ingredients = _ingredients.ToList(),
                    etapes = _etapes.ToList()
                };

                RecetteDTO res;

                // Vérification et envoi de l'image si elle existe
                if (!string.IsNullOrEmpty(txtImage.Text) && File.Exists(txtImage.Text))
                    res = await _deliceService.CreateRecetteAsync(createRecetteDTO, txtImage.Text);
                else
                    res = await _deliceService.CreateRecetteAsync(createRecetteDTO);

                // Actualisation de la liste et sélection de la nouvelle recette
                await ActualiserRecettes();
                await Task.Delay(100);
                BSRecettes.Position = _recettes.IndexOf(_recettes.FirstOrDefault(r => r.Id == res.Id));
                dgvGestionRecette.Refresh();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Modifier une recette existante
        private async void btModifierRecette_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (string.IsNullOrWhiteSpace(txtTitreRecette.Text) ||
                    listBoxDifficulte.SelectedItem == null ||
                    _ingredients.Count == 0 ||
                    _etapes.Count == 0)
                {
                    MessageBox.Show(
                        "Veuillez renseigner tous les champs requis : titre, difficulté, ingrédients et étapes.",
                        "Modification de recette",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                RecetteDTO current = BSRecettes.Current as RecetteDTO;

                // Récupération des catégories cochées
                List<CategorieDTO> listCategories = clbCategories.CheckedItems
                    .OfType<CategorieDTO>()
                    .ToList();

                // Création du DTO de mise à jour
                UpdateRecetteDTO updateDTO = new()
                {
                    Id = int.Parse(txtIdGestionrecette.Text),
                    nom = txtTitreRecette.Text,
                    temps_preparation = dtpTempsPreparation.Value.TimeOfDay,
                    temps_cuisson = dtpTempsCuisson.Value.TimeOfDay,
                    difficulte = int.Parse(listBoxDifficulte.Text),
                    categories = listCategories,
                    ingredients = _ingredients.ToList(),
                    etapes = _etapes.ToList()
                };

                RecetteDTO res;

                // Envoi avec ou sans image
                if (!string.IsNullOrEmpty(txtImage.Text) && File.Exists(txtImage.Text))
                    res = await _deliceService.UpdateRecetteAsync(updateDTO, txtImage.Text);
                else
                    res = await _deliceService.UpdateRecetteAsync(updateDTO);

                // Rafraîchissement et sélection de la recette modifiée
                txtImage.Clear();
                await ActualiserRecettes();
                await Task.Delay(100);
                
                BSRecettes.Position = _recettes.IndexOf(_recettes.FirstOrDefault(r => r.Id == res.Id));                

                MessageBox.Show("La recette a été mise à jour avec succès.", "Modification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la modification : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btSupprimerRecette_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor; // Change le curseur en "attente" pendant l'opération

            RecetteDTO current = BSRecettes.Current as RecetteDTO; // Récupère la recette actuellement sélectionnée dans le BindingSource

            if (current != null)
            {
                // Affiche une boîte de confirmation avant de supprimer la recette
                if ((MessageBox.Show(
                    $"Confirmez vous la suppression de la recette ?",
                    "Supprimer",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                {
                    await _deliceService.DeleteRecetteAsync(current.Id); // Appelle le service pour supprimer la recette

                    ActualiserRecettes(); // Rafraîchit la liste des recettes après suppression

                    // Affiche un message confirmant la suppression
                    MessageBox.Show(
                        "La recette sélectionnée a bien été supprimé.",
                        "Delete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        #endregion Fin gestion des recettes

        private void TabPagesAuthorizations()
        {
            // Si l'utilisateur n'est pas connecté
            if (!_deliceService.IsConnected())
            {
                // Parcourt tous les onglets sauvegardés (_tabPages) et retire tous sauf Login
                foreach (TabPage tab in _tabPages)
                {
                    if (tab == tabLogin)
                        continue; // On garde l'onglet Login

                    tabControl.TabPages.Remove(tab); // Supprime l'onglet du TabControl
                }

            }
            else // Si l'utilisateur est connecté
            {
                foreach (TabPage tab in _tabPages)
                {
                    // Si l'onglet n'est pas déjà présent dans le TabControl
                    if (!tabControl.TabPages.Contains(tab))
                    {
                        // On n'ajoute l'onglet Recettes que si l'utilisateur a le rôle Administrateur
                        if (tab != tabRecettes && !_roles.Contains("Administrateur"))
                            continue;

                        tabControl.TabPages.Add(tab); // Ajoute l'onglet autorisé
                    }
                }
            }
        }

        private void InitializeBinding()
        {
            // Initialisation des listes et des BindingSources
            _recettes = new();
            BSRecettes.DataSource = _recettes;
            dgvRecettes.DataSource = BSRecettes;

            // Masque certaines colonnes sensibles ou inutiles
            dgvRecettes.Columns[0].Visible = false;
            dgvRecettes.Columns[5].Visible = false;

            // Ajustement automatique de la colonne Nom
            dgvRecettes.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // Personnalisation de l'apparence de l'en-tête
            dgvRecettes.EnableHeadersVisualStyles = false;
            dgvRecettes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvRecettes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRecettes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvRecettes.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvRecettes.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Initialisation des catégories
            _categories = new();
            BSCategories.DataSource = _categories;
            dgvCategories.DataSource = BSCategories;

            // Liaison du TextBox txtNomCategories avec la propriété "nom" de BSCategories
            txtNomCategories.DataBindings.Add("Text", BSCategories, "nom", false, DataSourceUpdateMode.Never);

            // Initialisation des recettes par catégorie
            _recettesByCategorie = new();
            BSRecettesByCategorie.DataSource = _recettesByCategorie;

            // Initialisation des catégories par recette
            _categoriesByRecette = new();
            BSCategoriesByRecette.DataSource = _categoriesByRecette;

            // Personnalisation de l'apparence des DataGridView pour catégories
            dgvCategories.EnableHeadersVisualStyles = false;
            dgvCategories.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvCategories.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCategories.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvCategories.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvCategories.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Initialisation des relations recettes-catégories
            _recettesCategoriesRelations = new();
            BSRecettesCategoriesRelations.DataSource = _recettesCategoriesRelations;
            dgvRelationsRecCat.DataSource = _recettesCategoriesRelations;

            // Personnalisation de l'apparence des DataGridView pour relations
            dgvRelationsRecCat.EnableHeadersVisualStyles = false;
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(53, 155, 255);
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Configuration du DataGridView de gestion des recettes
            dgvGestionRecette.DataSource = BSRecettes;
            dgvGestionRecette.Columns[2].Visible = false;
            dgvGestionRecette.Columns[3].Visible = false;
            dgvGestionRecette.Columns[4].Visible = false;
            dgvGestionRecette.Columns[5].Visible = false;
            dgvGestionRecette.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvGestionRecette.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGestionRecette.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvGestionRecette.EnableHeadersVisualStyles = false;
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(53, 155, 255);
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Configuration du DataGridView des ingrédients
            dgvIngredientAjouter.AutoGenerateColumns = true;
            dgvIngredientAjouter.DataSource = _ingredients;
            dgvIngredientAjouter.EnableHeadersVisualStyles = false;
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(53, 155, 255);
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            // Configuration du DataGridView des étapes
            dgvEtapeAjouter.AutoGenerateColumns = true;
            dgvEtapeAjouter.DataSource = _etapes;
            dgvEtapeAjouter.Columns[0].Visible = false;
            dgvEtapeAjouter.EnableHeadersVisualStyles = false;
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }

        private async void ChangeDgvRByC()
        {
            // Met à jour le DataGridView pour afficher les recettes par catégorie
            dgvGetRecCat.DataSource = BSRecettesByCategorie;

            dgvGetRecCat.Columns[5].Visible = false;
            dgvGetRecCat.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGetRecCat.EnableHeadersVisualStyles = false;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }

        private async void ChangeDgvByR()
        {
            // Met à jour le DataGridView pour afficher les catégories par recette
            dgvGetRecCat.DataSource = BSCategoriesByRecette;

            dgvGetRecCat.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGetRecCat.EnableHeadersVisualStyles = false;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(53, 155, 255);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }

        private async void ChangeDgvByCategorie()
        {
            // Change le DataSource pour afficher uniquement les recettes filtrées par catégorie
            dgvRecettes.DataSource = BSRecettesByCategorie;
        }

        private async void ChangeBSRecettes()
        {
            // Restaure l'affichage global des recettes
            dgvRecettes.DataSource = BSRecettes;
        }
    }
}
