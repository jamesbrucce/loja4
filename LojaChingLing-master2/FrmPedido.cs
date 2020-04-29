using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LojaCL
{
    public partial class FrmPedido : Form
    {
        SqlConnection con = Conexao.obterConexao();
        public FrmPedido()
        {
            InitializeComponent();
        }

        static int botaocliacado = 0;

        public void CarregaCbxCartao()
        {
            String car = "SELECT * FROM cartaovenda";
            SqlCommand cmd = new SqlCommand(car, con);
            Conexao.obterConexao();
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(car, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "numero");
            cbxCartao.ValueMember = "Id";
            cbxCartao.DisplayMember = "numero";
            cbxCartao.DataSource = ds.Tables["numero"];
            Conexao.fecharConexao();
        }

        public void CarregaCbxProduto()
        {
            String pro = "SELECT Id,nome FROM produto";
            SqlCommand cmd = new SqlCommand(pro, con);
            Conexao.obterConexao();
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(pro, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "nome");
            cbxProduto.ValueMember = "Id";
            cbxProduto.DisplayMember = "nome";
            cbxProduto.DataSource = ds.Tables["nome"];
            Conexao.fecharConexao();
        }

        private void BtnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmPedido_Load(object sender, EventArgs e)
        {
            // assim que carregar o formulario todos esses comandos digitados false estará desabilitados 
            cbxProduto.Enabled = false;
            txtQuantidade.Enabled = false;
            btnNovoItem.Enabled = false;
            btnFinalizarPedido.Enabled = false;
            btnEcluirItem.Enabled = false;
            btnAtualizarPedido.Enabled = false;
            btnEditarItem.Enabled = false;
            txtUsuario.Enabled = false;
            txtValorUnitario.Enabled = false;
            CarregaCbxCartao();
            cbxCartao.Text = "";
        }

        private void CbxCartao_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("LocalizarCartaoVenda", con);
            cmd.Parameters.AddWithValue("@Id", cbxCartao.SelectedValue);
            cmd.CommandType = CommandType.StoredProcedure;
            Conexao.obterConexao();
            SqlDataReader rd = cmd.ExecuteReader();
            if(rd.Read())
            {
                txtUsuario.Text = rd["usuario"].ToString();
                Conexao.fecharConexao();
            }
            else
            {
                MessageBox.Show("Nenhum registro encontrado!", "Falha na pesquisa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Conexao.fecharConexao();
            }
        }

        private void BtnAbrirPedido_Click(object sender, EventArgs e)
        {
            cbxProduto.Enabled = true;
            txtQuantidade.Enabled = true;
            dgvPedido.Enabled = true;
            btnNovoItem.Enabled = true;
            btnFinalizarPedido.Enabled = true;
            btnEcluirItem.Enabled = true;
            btnEditarItem.Enabled = true;
            btnAtualizarPedido.Enabled = false;
            CarregaCbxProduto();
            cbxProduto.Text = "";
            dgvPedido.Columns.Add("ID", "IdProduto");
            dgvPedido.Columns.Add("Usuario", "Usuario");
            dgvPedido.Columns.Add("Produto", "Produto");
            dgvPedido.Columns.Add("Quantidade", "Quantidade");
            dgvPedido.Columns.Add("Valor", "Valor");
            dgvPedido.Columns.Add("Total", "Total");
        }

        private void CbxProduto_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("LocalizarProduto", con);
            cmd.Parameters.AddWithValue("@Id", cbxProduto.SelectedValue);
            cmd.CommandType = CommandType.StoredProcedure;
            Conexao.obterConexao();
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                txtValorUnitario.Text = rd["valor"].ToString();
                txtId.Text = rd["Id"].ToString();
                Conexao.fecharConexao();
            }
            else
            {
                MessageBox.Show("Nenhum registro encontrado!", "Falha na pesquisa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Conexao.fecharConexao();
            }
        }

        private void BtnNovoItem_Click(object sender, EventArgs e)
        {
            if (botaocliacado == 1)
            {
                SqlCommand pedido2 = new SqlCommand("InserirPedidos", con);
                pedido2.CommandType = CommandType.StoredProcedure;
                pedido2.Parameters.AddWithValue("@id_cartaovenda", SqlDbType.Int).Value = cbxCartao.SelectedValue;
                pedido2.Parameters.AddWithValue("@id_produto", SqlDbType.Int).Value = cbxProduto.SelectedValue;
                pedido2.Parameters.AddWithValue("@usuario", SqlDbType.NChar).Value = txtUsuario.Text;
                pedido2.Parameters.AddWithValue("@quantidade", SqlDbType.Int).Value =Convert.ToInt32(txtQuantidade.Text);
                pedido2.Parameters.AddWithValue("@dia_hora", SqlDbType.DateTime).Value = DateTime.Now;
                pedido2.Parameters.AddWithValue("@valor", SqlDbType.Int).Value = Convert.ToDecimal(txtValorUnitario.Text);
                pedido2.Parameters.AddWithValue("@total", SqlDbType.Int).Value = Convert.ToDecimal(txtQuantidade.Text) * Convert.ToDecimal(txtValorUnitario.Text);
                Conexao.obterConexao();
                pedido2.ExecuteNonQuery();
                Conexao.fecharConexao();
                MessageBox.Show("Pedido atualizado!", "atualizado pedido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                botaocliacado = 0;
            }
            else
            {
                DataGridViewRow item = new DataGridViewRow();
                item.CreateCells(dgvPedido);
                item.Cells[0].Value = txtId.Text;
                item.Cells[1].Value = txtUsuario.Text;
                item.Cells[2].Value = cbxProduto.Text;
                item.Cells[3].Value = txtQuantidade.Text;
                item.Cells[4].Value = txtValorUnitario.Text;
                item.Cells[5].Value = Convert.ToDecimal(txtValorUnitario.Text) * Convert.ToDecimal(txtQuantidade.Text);
                dgvPedido.Rows.Add(item);
                cbxProduto.Text = "";
                txtValorUnitario.Text = "";
                txtQuantidade.Text = "";
                decimal soma = 0;
                foreach (DataGridViewRow dr in dgvPedido.Rows)
                    soma += Convert.ToDecimal(dr.Cells[5].Value);
                txtValorTotal.Text = Convert.ToString(soma);

            }
        }

        private void DgvPedido_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.dgvPedido.Rows[e.RowIndex];
            cbxProduto.Text = row.Cells[2].Value.ToString();
            txtQuantidade.Text = row.Cells[3].Value.ToString();
            txtValorUnitario.Text = row.Cells[4].Value.ToString();
            int linha = dgvPedido.CurrentRow.Index;
        }

        private void BtnEditarItem_Click(object sender, EventArgs e)
        {
            int linha = dgvPedido.CurrentRow.Index;
            dgvPedido.Rows[linha].Cells[0].Value = txtId.Text;
            dgvPedido.Rows[linha].Cells[2].Value = cbxProduto.Text;
            dgvPedido.Rows[linha].Cells[3].Value = txtQuantidade.Text;
            dgvPedido.Rows[linha].Cells[4].Value = txtValorUnitario.Text;
            dgvPedido.Rows[linha].Cells[5].Value = Convert.ToDecimal(txtValorUnitario.Text) * Convert.ToDecimal(txtQuantidade.Text);
            decimal soma = 0;
            foreach (DataGridViewRow dr in dgvPedido.Rows)
                soma += Convert.ToDecimal(dr.Cells[5].Value);
            txtValorTotal.Text = Convert.ToString(soma);
            cbxProduto.Text = "";
            txtQuantidade.Text = "";
            txtValorUnitario.Text = "";

        }

        private void BtnEcluirItem_Click(object sender, EventArgs e)
        {
            int linha = dgvPedido.CurrentRow.Index;
            string query = "DELETE FROM pedido WHERE (id_cartaovenda = @id_cartaovenda AND id_produto = @id_produto)";
            SqlCommand cmd = new SqlCommand(query, con);
            DataGridViewRow row = dgvPedido.Rows[linha];
            cmd.Parameters.AddWithValue("@id_cartaovenda", cbxCartao.SelectedValue);
            cmd.Parameters.AddWithValue("@id_produto", row.Cells[0].Value);
            Conexao.obterConexao();
            cmd.ExecuteNonQuery();
            Conexao.fecharConexao();
            dgvPedido.Rows.RemoveAt(linha);
            dgvPedido.Refresh();
            decimal soma = 0;
            foreach (DataGridViewRow dr in dgvPedido.Rows)
                soma += Convert.ToDecimal(dr.Cells[5].Value);
            txtValorTotal.Text = Convert.ToString(soma);
            cbxProduto.Text = "";
            txtQuantidade.Text = "";
            txtValorUnitario.Text = "";
        }

        private void BtnFinalizarPedido_Click(object sender, EventArgs e)
        {
            Conexao.obterConexao();
            SqlCommand cartao = new SqlCommand("AtualizarStatusCartaoVenda",con);
            cartao.CommandType = CommandType.StoredProcedure;
            cartao.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = cbxCartao.SelectedValue;
            foreach (DataGridViewRow dr in dgvPedido.Rows)
            {
                SqlCommand pedidos = new SqlCommand("InserirPedidos", con);
                pedidos.CommandType = CommandType.StoredProcedure;
                pedidos.Parameters.AddWithValue("@id_cartaovenda", SqlDbType.Int).Value = cbxCartao.SelectedValue;
                pedidos.Parameters.AddWithValue("@id_produto", SqlDbType.Int).Value = Convert.ToInt32(dr.Cells[0].Value);
                pedidos.Parameters.AddWithValue("@usuario", SqlDbType.NChar).Value = dr.Cells[1].Value;
                pedidos.Parameters.AddWithValue("@quantidade", SqlDbType.Int).Value = Convert.ToInt32(dr.Cells[3].Value);
                pedidos.Parameters.AddWithValue("@dia_hora", SqlDbType.DateTime).Value = DateTime.Now;
                pedidos.Parameters.AddWithValue("@valor", SqlDbType.Int).Value = Convert.ToDecimal(dr.Cells[4].Value);
                pedidos.Parameters.AddWithValue("@total", SqlDbType.Int).Value = Convert.ToDecimal(dr.Cells[5].Value);
                Conexao.obterConexao();
                pedidos.ExecuteNonQuery();
                cartao.ExecuteNonQuery();
                Conexao.fecharConexao();
            }
            MessageBox.Show("Pedido realziado com sucesso!", "Pedido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            cbxProduto.Text = "";
            txtQuantidade.Text = "";
            txtValorUnitario.Text = "";
            txtValorTotal.Text = "";
            cbxProduto.Enabled = false;
            txtQuantidade.Enabled = false;
            txtValorUnitario.Enabled = false;
            btnNovoItem.Enabled = false;
            btnEditarItem.Enabled = false;
            btnEcluirItem.Enabled = false;
            btnFinalizarPedido.Enabled = false;
            //Limpar o DataGridPedido
            dgvPedido.Rows.Clear();
            dgvPedido.Refresh();
            FrmPrincipal obj = (FrmPrincipal)Application.OpenForms["FrmPrincipal"];
            obj.CarregadgvPripedi();

        }

        private void BtnLocalizar_Click(object sender, EventArgs e)
        {
            botaocliacado = 1;
            cbxProduto.Enabled = true;
            txtQuantidade.Enabled =true;
            btnNovoItem.Enabled =true;
            btnEcluirItem.Enabled = true;
            btnEditarItem.Enabled = true;
            btnFinalizarPedido.Enabled = false;
            btnAtualizarPedido.Enabled = true;
            CarregaCbxProduto();
            try
            {
                SqlConnection con = Conexao.obterConexao();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "LocalizarPedidoGrid";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cartaovenda", cbxCartao.SelectedValue);
                Conexao.obterConexao();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if(ds.Tables[0].Rows.Count > 0)
                {
                    dgvPedido.ReadOnly = true;
                    dgvPedido.DataSource = ds.Tables[0];
                    Conexao.fecharConexao();
                }
                else
                {
                    MessageBox.Show("Nenhum registro encontrado", "Falha", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Conexao.fecharConexao();
                }
                decimal soma = 0;
                foreach(DataGridViewRow dr in dgvPedido.Rows)
                {
                    soma += Convert.ToDecimal(dr.Cells[5].Value);
                    txtValorTotal.Text = Convert.ToString(soma);

                }

            }
            catch(Exception erro)
            {
                MessageBox.Show("Erro:" + erro);
            }
        }

        private void BtnAtualizarPedido_Click(object sender, EventArgs e)
        {
            Conexao.obterConexao();
            foreach(DataGridViewRow dr in dgvPedido.Rows)
            {
                SqlCommand pedidos = new SqlCommand("AtualizarPedidos", con);
                pedidos.CommandType = CommandType.StoredProcedure;
                pedidos.Parameters.AddWithValue("id_cartaovenda", SqlDbType.Int).Value = cbxCartao.SelectedValue;
                pedidos.Parameters.AddWithValue("id_produto", SqlDbType.Int).Value = Convert.ToInt32(dr.Cells[0].Value);
                pedidos.Parameters.AddWithValue("usuario", SqlDbType.NChar).Value = dr.Cells[1].Value;
                pedidos.Parameters.AddWithValue("quantidade", SqlDbType.Int).Value = Convert.ToInt32(dr.Cells[3].Value);
                pedidos.Parameters.AddWithValue("dia_hora", SqlDbType.DateTime).Value = DateTime.Now;
                pedidos.Parameters.AddWithValue("valor", SqlDbType.Decimal).Value = Convert.ToDecimal(dr.Cells[4].Value);
                pedidos.Parameters.AddWithValue("total", SqlDbType.Decimal).Value = Convert.ToDecimal(dr.Cells[5].Value);
                Conexao.obterConexao();
                pedidos.ExecuteNonQuery();
                Conexao.fecharConexao();
            }
            MessageBox.Show("Pedido atualizado com sucesso!", "atualizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            cbxProduto.Text = "";
            txtQuantidade.Text = "";
            txtValorUnitario.Text = "";
            txtValorTotal.Text = "";
            cbxProduto.Enabled = false;
            txtQuantidade.Enabled = false;
            txtValorUnitario.Enabled = false;
            txtValorTotal.Enabled = false;
            btnNovoItem.Enabled = false;
            btnEditarItem.Enabled = false;
            btnEcluirItem.Enabled = false;
            btnFinalizarPedido.Enabled = false;
            dgvPedido.Enabled = false;
            dgvPedido.DataSource = null;
            FrmPrincipal obj = (FrmPrincipal)Application.OpenForms["FrmPrincipal"];
            obj.CarregadgvPripedi();
        }
    }
}
