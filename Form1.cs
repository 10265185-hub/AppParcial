
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ControlProductos
{
    public partial class Form1 : Form
    {
    string cadenaConexion = @"Data Source=localhost\SQLEXPRESS04;Initial Catalog=ParcialProgramacion1;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public Form1()
        {
            InitializeComponent();

            btnGuardar.Click += btnGuardar_Click;
            btnConsultar.Click += btnConsultar_Click;
            btnActualizar.Click += btnActualizar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnLimpiar.Click += btnLimpiar_Click;
            dgvProductos.CellClick += dgvProductos_CellClick;

            CargarProductos();
        }

        // CONSULTAR
        private void CargarProductos()
        {
            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    string sql = "SELECT * FROM Productos";

                    SqlDataAdapter adaptador =
                        new SqlDataAdapter(sql, conexion);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    dgvProductos.DataSource = tabla;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // VALIDAR LOS DATOS
        private bool ValidarDatos()
        {
            decimal precio;
            int existencias;

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtCategoria.Text))
            {
                MessageBox.Show("Ingrese nombre y categoría.");
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text, out precio)
                || precio <= 0)
            {
                MessageBox.Show("El precio debe ser mayor que cero.");
                return false;
            }

            if (!int.TryParse(txtExistencias.Text, out existencias)
                || existencias < 0)
            {
                MessageBox.Show(
                    "Las existencias deben ser un entero mayor o igual a cero.");
                return false;
            }

            return true;
        }

        // GUARDAR
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos()) return;

            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    string sql = @"INSERT INTO Productos
                        (Nombre, Categoria, Precio, Existencias)
                        VALUES (@nombre, @categoria, @precio, @existencias)";

                    using (SqlCommand comando =
                        new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@nombre",
                            SqlDbType.NVarChar, 100).Value =
                            txtName.Text.Trim();

                        comando.Parameters.Add("@categoria",
                            SqlDbType.NVarChar, 50).Value =
                            txtCategoria.Text.Trim();

                        comando.Parameters.Add("@precio",
                            SqlDbType.Decimal).Value =
                            decimal.Parse(txtPrecio.Text);

                        comando.Parameters["@precio"].Precision = 10;
                        comando.Parameters["@precio"].Scale = 2;

                        comando.Parameters.Add("@existencias",
                            SqlDbType.Int).Value =
                            int.Parse(txtExistencias.Text);

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto guardado.");
                CargarProductos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // BOTÓN CONSULTAR
        private void btnConsultar_Click(object sender, EventArgs e)
        {
            CargarProductos();
        }

        // OBTENER EL ID DE LA FILA SELECCIONADA
        private int ObtenerIdSeleccionado()
        {
            if (dgvProductos.CurrentRow == null)
                return 0;

            return Convert.ToInt32(
                dgvProductos.CurrentRow.Cells["IdProducto"].Value);
        }

        // CARGAR DATOS AL SELECCIONAR UNA FILA
        private void dgvProductos_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

            txtName.Text =
                Convert.ToString(fila.Cells["Nombre"].Value);

            txtCategoria.Text =
                Convert.ToString(fila.Cells["Categoria"].Value);

            txtPrecio.Text =
                Convert.ToString(fila.Cells["Precio"].Value);

            txtExistencias.Text =
                Convert.ToString(fila.Cells["Existencias"].Value);
        }

        // ACTUALIZAR
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            int id = ObtenerIdSeleccionado();

            if (id == 0)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            if (!ValidarDatos()) return;

            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    string sql = @"UPDATE Productos SET
                        Nombre = @nombre,
                        Categoria = @categoria,
                        Precio = @precio,
                        Existencias = @existencias
                        WHERE IdProducto = @id";

                    using (SqlCommand comando =
                        new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@nombre",
                            SqlDbType.NVarChar, 100).Value =
                            txtName.Text.Trim();

                        comando.Parameters.Add("@categoria",
                            SqlDbType.NVarChar, 50).Value =
                            txtCategoria.Text.Trim();

                        comando.Parameters.Add("@precio",
                            SqlDbType.Decimal).Value =
                            decimal.Parse(txtPrecio.Text);

                        comando.Parameters["@precio"].Precision = 10;
                        comando.Parameters["@precio"].Scale = 2;

                        comando.Parameters.Add("@existencias",
                            SqlDbType.Int).Value =
                            int.Parse(txtExistencias.Text);

                        comando.Parameters.Add("@id",
                            SqlDbType.Int).Value = id;

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto actualizado.");
                CargarProductos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = ObtenerIdSeleccionado();

            if (id == 0)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Desea eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conexion =
                    new SqlConnection(cadenaConexion))
                {
                    string sql =
                        "DELETE FROM Productos WHERE IdProducto = @id";

                    using (SqlCommand comando =
                        new SqlCommand(sql, conexion))
                    {
                        comando.Parameters.Add("@id",
                            SqlDbType.Int).Value = id;

                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Producto eliminado.");
                CargarProductos();
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        // LIMPIAR
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtName.Clear();
            txtCategoria.Clear();
            txtPrecio.Clear();
            txtExistencias.Clear();

            dgvProductos.ClearSelection();
            txtName.Focus();
        }

        private void Nombre_Click(object sender, EventArgs e)
        {

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
