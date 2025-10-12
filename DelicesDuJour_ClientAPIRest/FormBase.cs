using DelicesDuJour_ClientAPIRest.Domain.DTO;
using DelicesDuJour_ClientAPIRest.Domain.DTO.CategorieDTOS;
using DelicesDuJour_ClientAPIRest.Domain.DTOS;
using DelicesDuJour_ClientAPIRest.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Windows.Forms;
using DelicesDuJour_ClientAPIRest.ControlUtilisateur;
using ReaLTaiizor.Controls;
using TabPage = System.Windows.Forms.TabPage;

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
        BindingList<CreateIngredientDTO> _ingredients = new();
        BindingList<CreateEtapeDTO> _etapes = new();

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
            tabControl.MouseMove += tabControl_MouseMove;
            tabControl.MouseLeave += tabControl_MouseLeave;


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

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
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

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
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

        private void tabControl_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabControl.TabCount; i++)
            {
                if (tabControl.GetTabRect(i).Contains(e.Location))
                {
                    _hoveredIndex = i;
                    tabControl.Invalidate();
                    return;
                }
            }
            _hoveredIndex = -1;
            tabControl.Invalidate();
        }

        private void tabControl_MouseLeave(object sender, EventArgs e)
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

        private void TabControl_VisibleChanged(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btLogOut_Click(object sender, EventArgs e)
        {
            _deliceService.Logout();

            _roles = [];
            //_recettes.Clear();
            //_authors.Clear();
            //_authorsByIdBook.Clear();
            //_authorsNotIdBook.Clear();

            lblRoles.Text = string.Empty;
            txtPassword.Text = string.Empty;

            TabPagesAuthorizations();
        }

        #endregion Fin Login

        #region recettes

        private async void btTtesRecettes_Click(object sender, EventArgs e)
        {
            ActualiserRecettes();
            ChangeDgv();
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

                // Étape 3 : vider et remplir la liste liée
                _recettesByCategorie.Clear();
                foreach (var recette in recettes)
                {
                    _recettesByCategorie.Add(recette);
                }
                ChangeDgvByCategorie();


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

                if (entreeCategorie == null)
                {
                    MessageBox.Show("La catégorie 'Plat' n’existe pas.", "Erreur",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                int idEntree = entreeCategorie.Id;

                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);

                //Étape 3 : vider et remplir la liste liée
                _recettesByCategorie.Clear();
                foreach (var recette in recettes)
                {
                    _recettesByCategorie.Add(recette);
                }
                ChangeDgvByCategorie();


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

                if (entreeCategorie == null)
                {
                    MessageBox.Show("La catégorie 'Dessert' n’existe pas.", "Erreur",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                int idEntree = entreeCategorie.Id;

                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);

                // Étape 3 : vider et remplir la liste liée
                _recettesByCategorie.Clear();
                foreach (var recette in recettes)
                {
                    _recettesByCategorie.Add(recette);
                }
                ChangeDgvByCategorie();


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

                if (entreeCategorie == null)
                {
                    MessageBox.Show("La catégorie 'Soupe' n’existe pas.", "Erreur",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Étape 2 : récupérer les recettes de cette catégorie via l’API
                int idEntree = entreeCategorie.Id;

                var recettes = await _deliceService.GetRecettesByIdCategorieAsync(idEntree);

                // Étape 3 : vider et remplir la liste liée
                _recettesByCategorie.Clear();
                foreach (var recette in recettes)
                {
                    _recettesByCategorie.Add(recette);
                }
                ChangeDgvByCategorie();


            }
            finally
            {
                Cursor = Cursors.Default;

            }
        }

        private void btDetailsRecette_Click(object sender, EventArgs e)
        {
            if (BSRecettes.Current is RecetteDTO currentRecette)
            {
                var formRecetteDetails = new FormRecetteDetails(currentRecette.Id);
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

            _recettes.Clear();
            foreach (RecetteDTO r in result)
                _recettes.Add(r);

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

                await ActualiserCategories();
                BSCategories.Position = _categories.IndexOf(_categories.FirstOrDefault(b => b.Id == res.Id));

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
                    await _deliceService.DeleteCategorieAsync(current.Id);
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

            _categories.Clear();
            foreach (CategorieDTO c in res)
                _categories.Add(c);

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


                // Étape 3 : vider et remplir la liste liée
                _recettesByCategorie.Clear();
                foreach (var r in recettes)
                {
                    _recettesByCategorie.Add(r);
                }
                ChangeDgvRByC();

            }
            finally
            {
                Cursor = Cursors.Default;

            }
        }

        private void btGetRecByCat_MouseMove(object sender, MouseEventArgs e)
        {
            btGetRecByCat.ForeColor = Color.FromArgb(182, 204, 254);
        }

        private void btGetRecByCat_MouseLeave(object sender, EventArgs e)
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

                // Étape 3 : vider et remplir la liste liée
                _categoriesByRecette.Clear();
                foreach (var c in categories)
                {
                    _categoriesByRecette.Add(c);
                }
                ChangeDgvByR();
            }
            finally
            {
                Cursor = Cursors.Default;

            }
        }

        private void btGetCatByRec_MouseHover(object sender, EventArgs e)
        {
            btGetCatByRec.ForeColor = Color.FromArgb(182, 204, 254);
        }

        private void btGetCatByRec_MouseLeave(object sender, EventArgs e)
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


                await _deliceService.DeleteRelationRecetteCategorieAsync(idCategorie, idRecette);

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

        private void btAjouterIngredient_Click(object sender, EventArgs e)
        {
            CreateIngredientDTO createIngredientDTO = new()
            {
                nom = txtNomIngredientAjouter.Text,
                quantite = txtQuantiteIngredientAjouter.Text
            };
            // Ajout à la liste liée au DataGridView
            _ingredients.Add(createIngredientDTO);

            dgvIngredientAjouter.Columns["Nom"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            // Optionnel : vider les champs après ajout
            txtNomIngredientAjouter.Clear();
            txtQuantiteIngredientAjouter.Clear();            
        }

        private void btEtapeAjouter_Click(object sender, EventArgs e)
        {
            CreateEtapeDTO createaEtapeDTO = new()
            {
                numero = int.Parse(txtNumeroEtapeAjouter.Text),
                titre = txtTitreEtapeAjouter.Text,
                texte = txtTexteEtapeAjouter.Text
            };
            // Ajout à la liste liée au DataGridView
            _etapes.Add(createaEtapeDTO);

            // Optionnel : vider les champs après ajout
            txtNumeroEtapeAjouter.Clear();
            txtTitreEtapeAjouter.Clear();
            txtTexteEtapeAjouter.Clear();

            dgvEtapeAjouter.Columns["Titre"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
        private async void btAjouterRecette_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                List<CategorieDTO> categoriesSelectionnees = new();

                foreach (var item in clbCategories.CheckedItems)
                {

                    if (item is CategorieDTO categorie)
                    {
                        categoriesSelectionnees.Add(categorie);
                    }

                }

                int difficulte = 1; // valeur par défaut
                if (listBoxDifficulte.SelectedItem != null)
                    difficulte = int.Parse(listBoxDifficulte.SelectedItem.ToString());


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

                var res = await _deliceService.CreateRecette(createRecetteDTO);

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

        private void ChangeDgvRByC()
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
        private void ChangeDgvByR()
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
        private void ChangeDgvByCategorie()
        {
            dgvRecettes.DataSource = BSRecettesByCategorie;
        }
        private void ChangeDgv()
        {
            dgvRecettes.DataSource = BSRecettes;

        }


    }
}
