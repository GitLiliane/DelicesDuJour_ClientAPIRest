using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using DelicesDuJour_ClientAPIRest.Domain.DTOS;
using DelicesDuJour_ClientAPIRest.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Forms;
using ReaLTaiizor.Controls;
using TabPage = System.Windows.Forms.TabPage;
using System.Threading.Tasks;

namespace DelicesDuJour_ClientAPIRest
{
    public partial class FormBase : Form
    {

        List<TabPage> _tabPages = [];
        IEnumerable<string> _roles = [];
        BindingList<RecetteDTO> _recettes;
        BindingList<CategorieDTO> _categories;
        BindingList<RecetteDTO> _recettesByCategorie;
        BindingList<CategorieDTO> _categoriesByRecette;
        BindingList<RecetteCategorieRelationshipDTO> _recettesCategoriesRelations;
        BindingList<IngredientDTO> _ingredients = new();
        BindingList<EtapeDTO> _etapes = new();
        BindingList<RecetteDTO> _gestionRecettes;

        private readonly DeliceService _deliceService = DeliceService.Instance;


        public FormBase()
        {
            InitializeComponent();

            // Active les boutons natifs
            metroControlBox1.MinimizeBox = true;
            metroControlBox1.MaximizeBox = true;
        }

        private async void FormBase_Load(object sender, EventArgs e)
        {
            InitializeBinding();

            _tabPages = tabControl.TabPages.Cast<TabPage>().ToList();
            txtHttp.Text = Properties.Settings.Default.Http;
            txtIdentifiant.Text = Properties.Settings.Default.Identifiant;
            TabPagesAuthorizations();

            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += tabControl_DrawItem;
            tabControl.MouseHover += tabControl_MouseHover;
            tabControl.MouseLeave += tabControl_MouseLeave;

            tabControl.SelectedTab = tabLogin;
            await Task.Delay(50);
            txtPassword.Focus();
            AcceptButton = btLogin;
        }
        private void FormBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("Confirmez-vous la fermeture de l'application ?", "Fermeture", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (res == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
        private void FormBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Http = txtHttp.Text;
            Properties.Settings.Default.Identifiant = txtIdentifiant.Text;
            Properties.Settings.Default.Save();

            btLogOut.PerformLayout();
        }

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
            }
        }

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

        // Ajoute ce code dans ton FormBase.cs

        private int _hoveredIndex = -1;

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

        private void tabControl_MouseHover(object sender, EventArgs e)
        {
            // Récupère la position de la souris dans le tabControl
            Point mousePos = tabControl.PointToClient(Cursor.Position);

            for (int i = 0; i < tabControl.TabCount; i++)
            {
                if (tabControl.GetTabRect(i).Contains(mousePos))
                {
                    if (_hoveredIndex != i)
                    {
                        _hoveredIndex = i;
                        tabControl.Invalidate();
                    }
                    return;
                }
            }

            // Si la souris ne survole aucun onglet
            if (_hoveredIndex != -1)
            {
                _hoveredIndex = -1;
                tabControl.Invalidate();
            }
        }

        private async void tabControl_MouseLeave(object sender, EventArgs e)
        {
            _hoveredIndex = -1;
            tabControl.Invalidate();
        }

        #region Login
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

                    TabPagesAuthorizations();

                    tabLogin.Parent = null;
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

        private async void btLogOut_Click(object sender, EventArgs e)
        {
            _deliceService.Logout();

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

            TabPagesAuthorizations();
        }

        #endregion Fin Login

        #region recettes

        private async void btTtesRecettes_Click(object sender, EventArgs e)
        {
            ActualiserRecettes();
            ChangeBSRecettes();
        }
        private async void btEntree_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Attendre que les catégories soient récupérées
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Entrée" dans _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Entrée", StringComparison.OrdinalIgnoreCase));

                int idEntree = entreeCategorie.Id;

                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);

                if (recettes.Any())
                {
                    // Étape 3 : vider et remplir la liste liée
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }
                    ChangeDgvByCategorie();
                }
                else
                {
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            finally
            {
                Cursor = Cursors.Default;

            }
        }

        private async void btPlat_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Attendre que les catégories soient récupérées
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Entrée" dans _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Plat", StringComparison.OrdinalIgnoreCase));

                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                int idEntree = entreeCategorie.Id;

                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);
                if (recettes.Any())
                {
                    //Étape 3 : vider et remplir la liste liée
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }
                    ChangeDgvByCategorie();
                }
                else
                {
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor = Cursors.Default;

            }
        }

        private async void btDessert_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Attendre que les catégories soient récupérées
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Entrée" dans _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Dessert", StringComparison.OrdinalIgnoreCase));

                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                int idEntree = entreeCategorie.Id;

                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);
                if (recettes.Any())
                {
                    // Étape 3 : vider et remplir la liste liée
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }
                    ChangeDgvByCategorie();
                }
                else
                {
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                Cursor = Cursors.Default;

            }
        }

        private async void btSoupe_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Attendre que les catégories soient récupérées
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Entrée" dans _categories
                var entreeCategorie = _categories
                    .FirstOrDefault(c => string.Equals(c.nom, "Soupe", StringComparison.OrdinalIgnoreCase));

                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                int idEntree = entreeCategorie.Id;

                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);

                if (recettes.Any())
                {
                    // Étape 3 : vider et remplir la liste liée
                    _recettesByCategorie.Clear();
                    foreach (var recette in recettes)
                    {
                        _recettesByCategorie.Add(recette);
                    }
                    ChangeDgvByCategorie();
                }
                else
                {
                    MessageBox.Show("Aucune recette trouvée dans cette catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            finally
            {
                Cursor = Cursors.Default;

            }
        }

        private async void btDetailsRecette_Click(object sender, EventArgs e)
        {
            if (BSRecettes.Current is RecetteDTO currentRecette)
            {
                var formRecetteDetails = new FormRecetteDetails(currentRecette.Id);
                formRecetteDetails.Show();
            }
            else if (BSRecettesByCategorie.Current is RecetteDTO current)
            {
                var formRecetteDetails = new FormRecetteDetails(current.Id);
                formRecetteDetails.Show();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une recette.",
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }            
        }

        private async Task ActualiserRecettes()
        {
            // Sauvegarde du current
            RecetteDTO current = BSRecettes.Current as RecetteDTO;

            // Remplissage de la liste
            var result = await _deliceService.GetRecettesAsync();

            if (result.Any())
            {
                _recettes.Clear();
                foreach (RecetteDTO r in result)
                    _recettes.Add(r);
            }
            else
            {
                MessageBox.Show("Aucune recette trouvée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // On se repositionne sur le current
            if (current is not null)
                BSRecettes.Position = _recettes.IndexOf(_recettes.Where(r => r.Id == current.Id).FirstOrDefault());
        }

        #endregion Fin Recettes

        #region Catégories

        private async void btActualiserCategorie_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                await ActualiserCategories();

            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btAjouterCategorie_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                CreateCategorieDTO createDTO = new()
                {
                    nom = txtNomCategories.Text,

                };

                var res = await _deliceService.AddCategorieAsync(createDTO);

                if (res != null)
                {
                    MessageBox.Show("La nouvelle catégorie a bien été ajouté", "Add", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private async void btModifierCategorie_Click(object sender, EventArgs e)
        {

            try
            {
                Cursor = Cursors.WaitCursor;
                CategorieDTO current = BSCategories.Current as CategorieDTO;

                if (current is not null)
                {
                    UpdateCategorieDTO updateDTO = new()
                    {
                        nom = txtNomCategories.Text
                    };

                    var res = await _deliceService.UpdateCategorieAsync(updateDTO, current.Id);

                    if (res != null)
                    {
                        MessageBox.Show("La catégorie a bien été modifié", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        await ActualiserCategories();
                        BSCategories.Position = _categories.IndexOf(_categories.FirstOrDefault(b => b.Id == res.Id));
                    }
                    else
                    {
                        MessageBox.Show("Une erreur s'est produite. Impossible de modifier la catégorie.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    await ActualiserCategories();
                }

            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btSupprimerCategorie_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                CategorieDTO current = BSCategories.Current as CategorieDTO;

                if (current is not null)
                {
                    if ((MessageBox.Show($"Confirmez vous la suppression de la catégorie {current.nom} ?", "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                    {
                        await _deliceService.DeleteCategorieAsync(current.Id);
                        MessageBox.Show("La catégorie a bien été supprimé", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    await ActualiserCategories();
                }

            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task ActualiserCategories()
        {
            // Sauvegarde du current
            CategorieDTO current = BSCategories.Current as CategorieDTO;

            // Remplissage de la liste
            var res = await _deliceService.GetCategoriesAsync();

            if (res.Any())
            {
                _categories.Clear();
                foreach (CategorieDTO c in res)
                    _categories.Add(c);
            }
            else
            {
                MessageBox.Show("Aucune catégorie trouvée.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // On se repositionne sur le current
            if (current is not null)
                BSCategories.Position = _categories.IndexOf(_categories.Where(b => b.Id == current.Id).FirstOrDefault());
        }



        #endregion Fin Categories


        #region Relation Recettes Catégories

        private async void btGetRecByCat_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Les catégories sont récupérées
                await ActualiserCategories();

                // Étape 1 : trouver la catégorie "Entrée" dans _categories
                var idCat = int.TryParse(txtIdCategorie.Text, out int id);

                var nomCat = txtNomCategorie.Text;

                var catId = _categories.FirstOrDefault(c => c.Id == id);

                var catNom = _categories.FirstOrDefault(c => c.nom == nomCat);

                int recette = 0;


                if (catId is not null)
                {
                    recette = catId.Id;
                }
                else if (catNom is not null)
                {
                    recette = catNom.Id;
                }


                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(recette);

                if (recettes.Any())
                {
                    // Étape 3 : vider et remplir la liste liée
                    _recettesByCategorie.Clear();
                    foreach (var r in recettes)
                    {
                        _recettesByCategorie.Add(r);
                    }
                    ChangeDgvRByC();
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

        private async void btGetRecByCat_MouseMove(object sender, MouseEventArgs e)
        {
            btGetRecByCat.ForeColor = Color.FromArgb(182, 204, 254);
        }

        private async void btGetRecByCat_MouseLeave(object sender, EventArgs e)
        {
            btGetRecByCat.ForeColor = Color.WhiteSmoke;
        }

        private async void btGetCatByRec_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Les recettes sont récupérées                
                await ActualiserRecettes();

                // Étape 1 : trouver les catégories dans _recettess
                var idRec = int.TryParse(txtIdrecette.Text, out int id);

                var nomRec = txtNomRecette.Text;

                var recId = _recettes.FirstOrDefault(c => c.Id == id);

                var recNom = _recettes.FirstOrDefault(c => c.nom == nomRec);

                int categorie = 0;


                if (recId is not null)
                {
                    categorie = recId.Id;
                }
                else if (recNom is not null)
                {
                    categorie = recNom.Id;
                }


                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                var categories = await _deliceService.GetCategoriesByIdRecette(categorie);

                if (categories.Any())
                {// Étape 3 : vider et remplir la liste liée
                    _categoriesByRecette.Clear();
                    foreach (var c in categories)
                    {
                        _categoriesByRecette.Add(c);
                    }
                    ChangeDgvByR();
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

        private async void btGetCatByRec_MouseHover(object sender, EventArgs e)
        {
            btGetCatByRec.ForeColor = Color.FromArgb(182, 204, 254);
        }

        private async void btGetCatByRec_MouseLeave(object sender, EventArgs e)
        {
            btGetCatByRec.ForeColor = Color.WhiteSmoke;
        }


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
                BSRecettesCategoriesRelations.Position = _recettesCategoriesRelations.IndexOf(_recettesCategoriesRelations.FirstOrDefault(r => r.idRecette == idRecette));
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btSupprimerRelationRecCat_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                int idRecette = int.Parse(txtRelationIdRecette.Text);
                int idCategorie = int.Parse(txtRelationIdCategorie.Text);


                if ((MessageBox.Show($"Confirmez vous la suppression de la relation ?", "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                {
                    await _deliceService.DeleteRelationRecetteCategorieAsync(idCategorie, idRecette);
                    MessageBox.Show("La relation a bien été supprimé", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await ActualiserRecettesCategoriesRelations();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private async Task ActualiserRecettesCategoriesRelations()
        {
            // Sauvegarde du current
            RecetteCategorieRelationshipDTO current = BSRecettesCategoriesRelations.Current as RecetteCategorieRelationshipDTO;

            // Remplissage de la liste
            var res = await _deliceService.GetRecetteCategorieRelationshipsAsync();

            _recettesCategoriesRelations.Clear();
            foreach (RecetteCategorieRelationshipDTO r in res)
                _recettesCategoriesRelations.Add(r);

            // On se repositionne sur le current
            if (current is not null)
                BSRecettesCategoriesRelations.Position = _recettesCategoriesRelations.IndexOf(_recettesCategoriesRelations.Where(r => r.idRecette == current.idRecette).FirstOrDefault());
        }
        #endregion Fin Relation Recettes Catégories

        #region gestion des recettes

        private async void dgvGestionRecette_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            RecetteDTO current = BSRecettes.Current as RecetteDTO;

            if (current is not null)
            {
                var currentRecette = await _deliceService.GetRecetteByIdAsync(current.Id);
                if (currentRecette is not null)
                {
                    txtIdGestionrecette.Text = currentRecette.Id.ToString();
                    txtTitreRecette.Text = currentRecette.nom;
                    dtpTempsPreparation.Value = DateTime.Today.Add(currentRecette.temps_preparation);
                    dtpTempsCuisson.Value = DateTime.Today.Add(currentRecette.temps_cuisson);
                    listBoxDifficulte.SelectedItem = currentRecette.difficulte.ToString();

                    for (int i = 0; i < clbCategories.Items.Count; i++)
                    {
                        clbCategories.SetItemChecked(i, false);
                    }

                    // Parcourt chaque catégorie de la recette
                    foreach (var categorie in currentRecette.categories)
                    {
                        // Parcourt les items du CheckedListBox
                        for (int i = 0; i < clbCategories.Items.Count; i++)
                        {
                            // Récupère la catégorie affichée dans la liste
                            var item = clbCategories.Items[i] as CategorieDTO;

                            // Compare par Id ou par Nom
                            if (item != null && item.Id == categorie.Id)
                            {
                                clbCategories.SetItemChecked(i, true);
                            }
                        }
                    }

                    _ingredients.Clear();

                    foreach (IngredientDTO ingredient in currentRecette.ingredients)
                    {
                        _ingredients.Add(ingredient);
                    }

                    _etapes.Clear();

                    foreach (EtapeDTO etape in currentRecette.etapes)
                    {
                        _etapes.Add(etape);
                    }

                }
            }


        }

        private async void btAjouterIngredient_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNomIngredientAjouter.Text) && !string.IsNullOrEmpty(txtQuantiteIngredientAjouter.Text))
            {
                IngredientDTO IngredientDTO = new()
                {
                    nom = txtNomIngredientAjouter.Text,
                    quantite = txtQuantiteIngredientAjouter.Text
                };
                // Ajout à la liste liée au DataGridView
                _ingredients.Add(IngredientDTO);
            }
            else
            {
                MessageBox.Show("Veuillez renseigner à la fois le nom et la quantité.", "Ajout de catégorie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }           

            dgvIngredientAjouter.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // Optionnel : vider les champs après ajout
            txtNomIngredientAjouter.Clear();
            txtQuantiteIngredientAjouter.Clear();
        }

        private async void btSupprimerIngredient_Click(object sender, EventArgs e)
        {
            if (dgvIngredientAjouter.CurrentRow != null)
            {
                var current = dgvIngredientAjouter.CurrentRow.DataBoundItem as IngredientDTO;

                if ((MessageBox.Show($"Confirmez vous la suppression de l'ingrédient ?", "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                {
                    _ingredients.Remove(current);

                    MessageBox.Show("L'ingredient sélectionné a bien été supprimé.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
            
        }

        private async void btEtapeAjouter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNumeroEtapeAjouter.Text) && !string.IsNullOrEmpty(txtTitreEtapeAjouter.Text) && !string.IsNullOrEmpty(txtTexteEtapeAjouter.Text))
            {
                EtapeDTO EtapeDTO = new()
                {
                    numero = int.Parse(txtNumeroEtapeAjouter.Text),
                    titre = txtTitreEtapeAjouter.Text,
                    texte = txtTexteEtapeAjouter.Text
                };
                // Ajout à la liste liée au DataGridView
                _etapes.Add(EtapeDTO);

                // Optionnel : vider les champs après ajout
                txtNumeroEtapeAjouter.Clear();
                txtTitreEtapeAjouter.Clear();
                txtTexteEtapeAjouter.Clear();
            }
            else
            {
                MessageBox.Show("Veuillez renseigner à la fois le numéro, le nom et le texte de l'étape.", "Ajout d'étape'", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            dgvEtapeAjouter.Columns["Titre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private async void btEtapeSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvEtapeAjouter.CurrentRow != null)
            {
                if ((MessageBox.Show($"Confirmez vous la suppression de l'étape ?", "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                {
                    var current = dgvEtapeAjouter.CurrentRow.DataBoundItem as EtapeDTO;
                    _etapes.Remove(current);

                    MessageBox.Show("L'étape sélectionnée a bien été supprimé.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
        }
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
                }

                else
                {
                    // Récupération des catégories cochées
                    List<CategorieDTO> categoriesSelectionnees = new();
                    foreach (var item in clbCategories.CheckedItems)
                    {
                        if (item is CategorieDTO categorie)
                            categoriesSelectionnees.Add(categorie);
                    }

                    // Difficulté
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

                    // Création de la recette
                    RecetteDTO res = await _deliceService.CreateRecetteAsync(createRecetteDTO);

                    // Actualisation
                    await ActualiserRecettes();
                    await Task.Delay(100);
                    BSRecettes.Position = _recettes.IndexOf(_recettes.FirstOrDefault(r => r.Id == res.Id));
                    dgvGestionRecette.Refresh();
                }                    
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btModifierRecette_Click(object sender, EventArgs e)
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
                }
                else
                {
                    RecetteDTO current = BSRecettes.Current as RecetteDTO;

                    List<CategorieDTO> listCategories = new();

                    foreach (var item in clbCategories.CheckedItems)
                    {
                        if (item is CategorieDTO categorie)
                        {
                            listCategories.Add(categorie);
                        }
                    }

                    UpdateRecetteDTO updateDTO = new();
                    if (current is not null)
                    {
                        if (txtIdGestionrecette is not null && listBoxDifficulte.SelectedItem is not null)
                        {
                            updateDTO.Id = int.Parse(txtIdGestionrecette.Text);
                            updateDTO.nom = txtTitreRecette.Text;
                            updateDTO.temps_preparation = dtpTempsPreparation.Value.TimeOfDay;
                            updateDTO.temps_cuisson = dtpTempsCuisson.Value.TimeOfDay;
                            updateDTO.difficulte = int.Parse(listBoxDifficulte.Text);
                            updateDTO.categories = listCategories;
                            updateDTO.ingredients = _ingredients.ToList();
                            updateDTO.etapes = _etapes.ToList();
                        }
                    }

                    await _deliceService.UpdateRecetteAsync(updateDTO);

                    ActualiserRecettes();
                }          

            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private async void btSupprimerRecette_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            RecetteDTO current = BSRecettes.Current as RecetteDTO;

            if (current != null)
            {
                if ((MessageBox.Show($"Confirmez vous la suppression de la recette ?", "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                {
                    await _deliceService.DeleteRecetteAsync(current.Id);

                    ActualiserRecettes();

                    MessageBox.Show("La recette sélectionnée a bien été supprimé.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }            
        }
        #endregion Fin gestion des recettes
        private void TabPagesAuthorizations()
        {
            //LogOut
            if (!_deliceService.IsConnected())
            {
                // Parcourt les onglets et retire tous sauf Login
                foreach (TabPage tab in _tabPages)
                {
                    if (tab == tabLogin)
                        continue;

                    tabControl.TabPages.Remove(tab);
                }

            }
            else // LogIn
            {
                foreach (TabPage tab in _tabPages)
                {
                    if (!tabControl.TabPages.Contains(tab))
                    {
                        if (tab != tabRecettes && !_roles.Contains("Administrateur"))
                            continue;

                        tabControl.TabPages.Add(tab);
                    }
                }
            }
        }

        private void InitializeBinding()
        {
            _recettes = new();
            BSRecettes.DataSource = _recettes;
            dgvRecettes.DataSource = BSRecettes;

            dgvRecettes.Columns[0].Visible = false;
            dgvRecettes.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvRecettes.EnableHeadersVisualStyles = false;
            dgvRecettes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvRecettes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRecettes.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvRecettes.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvRecettes.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            _categories = new();
            BSCategories.DataSource = _categories;
            dgvCategories.DataSource = BSCategories;
            txtNomCategories.DataBindings.Add("Text", BSCategories, "nom", false, DataSourceUpdateMode.Never);

            _recettesByCategorie = new();
            BSRecettesByCategorie.DataSource = _recettesByCategorie;

            _categoriesByRecette = new();
            BSCategoriesByRecette.DataSource = _categoriesByRecette;

            dgvCategories.EnableHeadersVisualStyles = false;
            dgvCategories.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvCategories.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCategories.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvCategories.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvCategories.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;



            _recettesCategoriesRelations = new();
            BSRecettesCategoriesRelations.DataSource = _recettesCategoriesRelations;
            dgvRelationsRecCat.DataSource = _recettesCategoriesRelations;

            dgvRelationsRecCat.EnableHeadersVisualStyles = false;
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvRelationsRecCat.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvGestionRecette.DataSource = BSRecettes;
            dgvGestionRecette.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGestionRecette.EnableHeadersVisualStyles = false;
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvGestionRecette.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvIngredientAjouter.AutoGenerateColumns = true;
            dgvIngredientAjouter.DataSource = _ingredients;

            dgvIngredientAjouter.EnableHeadersVisualStyles = false;
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvIngredientAjouter.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvEtapeAjouter.AutoGenerateColumns = true;
            dgvEtapeAjouter.DataSource = _etapes;

            dgvEtapeAjouter.EnableHeadersVisualStyles = false;
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvEtapeAjouter.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

        }

        private async void ChangeDgvRByC()
        {
            dgvGetRecCat.DataSource = BSRecettesByCategorie;

            dgvGetRecCat.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGetRecCat.EnableHeadersVisualStyles = false;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }
        private async void ChangeDgvByR()
        {
            dgvGetRecCat.DataSource = BSCategoriesByRecette;

            dgvGetRecCat.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvGetRecCat.EnableHeadersVisualStyles = false;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 124, 204);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(154, 205, 255);
            dgvGetRecCat.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }
        private async void ChangeDgvByCategorie()
        {
            dgvRecettes.DataSource = BSRecettesByCategorie;
        }
        private async void ChangeBSRecettes()
        {
            dgvRecettes.DataSource = BSRecettes;
        }


    }
}
