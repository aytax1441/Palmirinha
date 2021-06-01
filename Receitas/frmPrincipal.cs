using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Receitas
{
    public partial class frmPrincipal : Form
    {
        bool adicionando = false, registrando = false;
        List<receitas> listaReceitas = new();
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void carregaGrid()
        {
            StreamReader sr = File.OpenText("receitas.dat");
            string s;
            gridDados.Rows.Clear();
            listaReceitas = new();
            while ((s = sr.ReadLine()) != null)
            {
                string decodificado = criptografia.Decrypt(s);
                string[] repartida = decodificado.Split(" -|- ");
                repartida[5] = repartida[5].Replace("-;-", Environment.NewLine);
                listaReceitas.Add(new receitas(repartida[0], repartida[1], repartida[2], repartida[3], repartida[4], repartida[5]));
            }

            listaReceitas = listaReceitas.OrderBy(x => x.codigo).ToList();
            foreach(receitas r in listaReceitas)
            {
                gridDados.Rows.Add(r.codigo, r.titulo, r.autor);
            }

            sr.Close();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {

            if (!File.Exists("receitas.dat"))
            {
                StreamWriter sw = File.CreateText("receitas.dat");
                sw.Close();
            }
            carregaGrid();
        }

        private void btPesquisar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            var objetoFinal = new List<receitas>();
            if (cbFiltro.SelectedIndex == 0)
            {
                //Filtro por código
                int codigo = 0;
                try
                {
                    codigo = Convert.ToInt32(txtFiltro.Text);
                }catch(Exception ex)
                {
                    errorProvider1.SetError(txtFiltro, "Filtro inválido...");
                    return;
                }

                var objeto = from v in listaReceitas where v.codigo == codigo select v;
                objetoFinal = objeto.ToList<receitas>();

            }
            else if(cbFiltro.SelectedIndex == 1)
            {
                //Filtro por Título
                string titulo = "";
                try
                {
                    titulo = txtFiltro.Text.ToLower();
                }
                catch (Exception ex)
                {
                    errorProvider1.SetError(txtFiltro, "Filtro inválido...");
                    return;
                }

                var objeto = listaReceitas.Where(x => x.titulo.ToLower().Contains(titulo)).Select(x => x);
                objetoFinal = objeto.ToList<receitas>();

            }
            else if(cbFiltro.SelectedIndex == 2)
            {
                //Filtro por Autor
                string autor = "";
                try
                {
                    autor = txtFiltro.Text.ToLower(); ;
                }
                catch (Exception ex)
                {
                    errorProvider1.Clear();
                    errorProvider1.SetError(txtFiltro, "Filtro inválido...");
                    return;
                }

                var objeto = listaReceitas.Where(x => x.autor.ToLower().Contains(autor)).Select(x => x);
                objetoFinal = objeto.ToList<receitas>();
            }
            else
            {
                //Retornar erro por filtro inválido
                errorProvider1.SetError(txtFiltro, "Favor selecionar uma opção de filtro.");
                return;
            }

            
            if (objetoFinal.Count > 0)
            {
                gridDados.Rows.Clear();
                foreach (receitas dados in objetoFinal)
                {
                    
                    gridDados.Rows.Add(dados.codigo, dados.titulo, dados.autor);
                }
            }
            else
            {
                MessageBox.Show("Nenhum registro encontrado.", "Busca inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void gridDados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Se não for um valor inválido de linha (tipo o header)
            if(e.RowIndex >= 0)
            {
                //Busca o código em específico...
                int codigo = Convert.ToInt32(gridDados.Rows[e.RowIndex].Cells[0].Value);
                var objeto = from v in listaReceitas where v.codigo == codigo select v;
                List<receitas> objetoFinal = objeto.ToList<receitas>();
                //Define os valores da tela de cadastro para utilização
                txtCodigo.Text = objetoFinal[0].codigo.ToString();
                txtAutor.Text = objetoFinal[0].autor.ToString();
                txtTitulo.Text = objetoFinal[0].titulo.ToString();
                dtaCriacao.Text = objetoFinal[0].dta_criacao.ToString();
                dtaAlteracao.Text = objetoFinal[0].dta_alteracao.ToString();
                txtReceita.Text = objetoFinal[0].receita.ToString();

                //Alterna para a tela de cadastro
                tc1.SelectedTab = tbCadastro;
            }
        }

        private void btnAlterar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            if (txtCodigo.Text != "")
            {
                habilitarBotoes(false);
                registrando = true;
            }
            else
                errorProvider1.SetError(btnAlterar, "Nenhum registro selecionado!");
        }

        private void tbPesquisa_Click(object sender, EventArgs e)
        {
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Se estiver adicionando um valor, limpa os campos;
            // Se estiver alterando um valor, volta o registro à como era antes.
            if (adicionando)
            {
                limpaCampos();
            }
            else
            {
                // Carregar novamente os dados que estão selecionados
                var objeto = from v in listaReceitas where v.codigo == Convert.ToInt32(txtCodigo.Text) select v;
                List<receitas> objetoFinal = objeto.ToList<receitas>();

                txtCodigo.Text = objetoFinal[0].codigo.ToString();
                txtAutor.Text = objetoFinal[0].autor.ToString();
                txtTitulo.Text = objetoFinal[0].titulo.ToString();
                dtaCriacao.Text = objetoFinal[0].dta_criacao.ToString();
                dtaAlteracao.Text = objetoFinal[0].dta_alteracao.ToString();
                txtReceita.Text = objetoFinal[0].receita.ToString();
            }
            habilitarBotoes(true);
            adicionando = false;
            registrando = false;
        }

        private void habilitarBotoes(bool enabled)
        {
            btnAdicionar.Enabled = enabled;
            btnExcluir.Enabled = enabled;
            btnAlterar.Enabled = enabled;
            foreach (Control ctl in tbPesquisa.Controls) ctl.Enabled = enabled;

            btnCancelar.Enabled = !enabled;
            btnSalvar.Enabled = !enabled;

            txtAutor.Enabled = !enabled;
            txtTitulo.Enabled = !enabled;
            txtReceita.Enabled = !enabled;
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            adicionarReceita();
        }

        private void adicionarReceita()
        {
            int codigo;
            errorProvider1.Clear();
            try
            {
                // Ordenar a lista de receitas
                listaReceitas = listaReceitas.OrderBy(x => x.codigo).ToList();

                // Buscar o último código
                int total = listaReceitas.Count;

                // Criar variável com o código a ser usado
                codigo = listaReceitas[total - 1].codigo + 1;
            }
            catch
            {
                codigo = 1;
            }
            // Limpando os campos
            limpaCampos();

            // Definindo o código
            txtCodigo.Text = codigo.ToString();

            // Desabilitar os botões e entrar em modo de inserção
            habilitarBotoes(false);
            adicionando = true;
            registrando = true;
        }
        private void limpaCampos()
        {
            txtCodigo.Clear();
            txtAutor.Clear();
            txtTitulo.Clear();
            txtReceita.Clear();
            dtaAlteracao.Text = "";
            dtaCriacao.Text = "";
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            // Verifica se todos os campos estão preenchidos
            errorProvider1.Clear();
            bool erro = false;
            if (txtAutor.Text == "")
            {
                errorProvider1.SetError(txtAutor, "Preenchimento obrigatório!");
                erro = true;
            }
            if (txtTitulo.Text == "")
            {
                errorProvider1.SetError(txtTitulo, "Preenchimento obrigatório!");
                erro = true;
            }
            if (txtReceita.Text == "")
            {
                errorProvider1.SetError(txtReceita, "Preenchimento obrigatório!");
                erro = true;
            }
            if (!erro)
                salvarReceita();
        }

        private void salvarReceita()
        {
            // Se estiver adicionando, irá inserir o registro no arquivo
            if (adicionando)
            {
                using (StreamWriter sw = new("receitas.dat"))
                {
                    foreach (receitas x in listaReceitas)
                    {
                        // Substituindo a quebra de linha por um trecho que será
                        // trocado depois por Environment.NewLine
                        x.receita = x.receita.Replace(Environment.NewLine, "-;-");
                        sw.WriteLine(criptografia.Encrypt(x.codigo + " -|- " + x.titulo + " -|- " + x.autor + " -|- " + x.dta_criacao + " -|- " + x.dta_alteracao + " -|- " + x.receita));
                    }
                    string receita;
                    receita = txtReceita.Text.Replace(Environment.NewLine, "-;-");
                    sw.WriteLine(criptografia.Encrypt(txtCodigo.Text + " -|- " + txtTitulo.Text + " -|- " + txtAutor.Text + " -|- " + DateTime.Now.ToString() + " -|- " + DateTime.Now.ToString() + " -|- " + receita));
                    sw.Close();
                    sw.Dispose();
                }
                MessageBox.Show("Registro adicionado com sucesso!", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            } // Senão, irá alterar o registro escolhido
            else
            {
                using (StreamWriter sw = new("receitas.dat"))
                {
                    foreach (receitas x in listaReceitas)
                    {
                        string receita;
                        // Substituindo a quebra de linha por um trecho que será
                        // trocado depois por Environment.NewLine
                        if (x.codigo == Convert.ToInt32(txtCodigo.Text))
                        {
                            receita = txtReceita.Text.Replace(Environment.NewLine, "-;-");
                            sw.WriteLine(criptografia.Encrypt(txtCodigo.Text + " -|- " + txtTitulo.Text + " -|- " + txtAutor.Text + " -|- " + dtaCriacao.Text + " -|- " + DateTime.Now.ToString() + " -|- " + receita));
                        }
                        else
                        {
                            receita = x.receita.Replace(Environment.NewLine, "-;-");
                            sw.WriteLine(criptografia.Encrypt(x.codigo + " -|- " + x.titulo + " -|- " + x.autor + " -|- " + x.dta_criacao + " -|- " + x.dta_alteracao + " -|- " + receita));
                        }
                    }
                    sw.Close();
                    sw.Dispose();
                }
                MessageBox.Show("Registro alterado com sucesso!", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            limpaCampos();
            adicionando = false;
            registrando = false;
            habilitarBotoes(true);
            carregaGrid();
            tc1.SelectedTab = tbPesquisa;
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            excluirReceita();
        }

        private void excluirReceita()
        {
            errorProvider1.Clear();
            if (!adicionando && !registrando && txtCodigo.Text != "")
            {
                var pergunta = MessageBox.Show("Essa ação irá excluir a receita de código " + txtCodigo.Text + "." + Environment.NewLine + Environment.NewLine + "Deseja realmente prosseguir?", "Cuidado!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (pergunta == DialogResult.Yes)
                {
                    using (StreamWriter sw = new("receitas.dat"))
                    {
                        foreach (receitas x in listaReceitas)
                        {
                            string receita;
                            // Substituindo a quebra de linha por um trecho que será
                            // trocado depois por Environment.NewLine
                            if (x.codigo != Convert.ToInt32(txtCodigo.Text))
                            {

                                receita = x.receita.Replace(Environment.NewLine, "-;-");
                                sw.WriteLine(criptografia.Encrypt(x.codigo + " -|- " + x.titulo + " -|- " + x.autor + " -|- " + x.dta_criacao + " -|- " + x.dta_alteracao + " -|- " + receita));
                            }
                        }
                        sw.Close();
                        sw.Dispose();
                    }
                    MessageBox.Show("Registro excluído com sucesso!", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    limpaCampos();
                    habilitarBotoes(true);
                    carregaGrid();
                    tc1.SelectedTab = tbPesquisa;
                }
            }
        }
        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (adicionando || registrando)
            {
                var pergunta = MessageBox.Show("Você está alterando/adicionando um registro atualmente." + Environment.NewLine + Environment.NewLine + "Deseja realmente encerrar a aplicação?", "Atenção!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                e.Cancel = (pergunta == DialogResult.No);
            }
        }
    }
}
