using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using VehicleOrderMasterDetail.ViewModel;

namespace VehicleOrderMasterDetail
{
    public partial class Form1 : Form
    {
        int intSlNo = 0;
        bool isDefaultImage = true;
        string conStr = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        string strPreviousImage = "";
        OpenFileDialog ofd = new OpenFileDialog();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadVehicleType();
            ClearAll();
            LoadOrderListGrid();

        }

        private void LoadOrderListGrid()
        {
            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {

                sqlCon.Open();

                SqlDataAdapter da = new SqlDataAdapter("ViewAllOrder",sqlCon);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                dt.Columns.Add("Image", Type.GetType("System.Byte[]"));
                foreach (DataRow dr in dt.Rows)

                {
                    try
                    {
                        dr["Image"] = File.ReadAllBytes(Application.StartupPath + "\\Images\\" + dr["ImagePath"].ToString());
                    }
                    catch
                    {
                        dr["Image"] = File.ReadAllBytes(Application.StartupPath + "\\Images\\noimage.png");
                    }


                }
                dgvOrderList.RowTemplate.Height = 80;
                dgvOrderList.DataSource = dt;
                ((DataGridViewImageColumn)dgvOrderList.Columns[dgvOrderList.ColumnCount - 1]).ImageLayout = DataGridViewImageCellLayout.Stretch;
                da.Dispose();
            }
        }

        private void ClearAll()
        {
            txtOrderNo.Text = txtFirstName.Text =txtLastName.Text ="";
           
            cmbVehicleType.SelectedIndex = 0;
            dtpDateOfOrder.Value = DateTime.Now;
            if (dgvDetails.DataSource == null)
            {
                dgvDetails.Rows.Clear();
            }
            else
            {
                dgvDetails.DataSource = (dgvDetails.DataSource as DataTable).Clone();
            }
            intSlNo = 0;
            btnSave.Text = "Save";
            btnDelete.Enabled = false;
            imgImage.Image = Image.FromFile(Application.StartupPath + "\\Images\\noimage.png");
            isDefaultImage = true;
            chkIsRegular.Checked = false;
            rbtnNew.Checked = true;
        }

        private void LoadVehicleType()
        {
            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                sqlCon.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM VehicleType", sqlCon);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataRow topRow = dt.NewRow();
                topRow[0] = 0;
                topRow[1] = "-- Select --";
                dt.Rows.InsertAt(topRow, 0);
                cmbVehicleType.DataSource = dt;
                cmbVehicleType.ValueMember = "TypeId";
                cmbVehicleType.DisplayMember = "Type";
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Images(.jpg,.png)| *.png;*.jpg";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imgImage.Image = new Bitmap(ofd.FileName);
                isDefaultImage = false;
                strPreviousImage = "";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            imgImage.Image = Image.FromFile(Application.StartupPath + "\\Images\\noimage.png");
            isDefaultImage = true;
            strPreviousImage = "";
        }
        string SaveImage(string fileName)
        {
            string imageName = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            imageName = imageName.Length <= 15 ? imageName : imageName.Substring(0, 15);
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + extension;
            imgImage.Image.Save(Application.StartupPath + "\\Images\\" + imageName);
            return imageName;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearAll();
        }
        bool ValidateForm()
        {
            bool isValid = true;
            if (txtOrderNo.Text.Trim() == "")
            {
                isValid = false;
                MessageBox.Show("Order No required");
            }
            if (txtFirstName.Text.Trim() == "")
            {
                isValid = false;
                MessageBox.Show("First name required");
            }
            if (txtLastName.Text.Trim() == "")
            {
                isValid = false;
                MessageBox.Show("First name required");
            }
            if (cmbVehicleType.SelectedIndex == 0)
            {
                isValid = false;
                MessageBox.Show("Select Vehicle Type");
            }
            return isValid;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                int slNo = 0;
                using (SqlConnection sqlCon = new SqlConnection(conStr))
                {
                    sqlCon.Open();
                    using (SqlTransaction tran = sqlCon.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand sqlCmd = new SqlCommand("OrderInfoAddOrEdit", sqlCon, tran);
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.AddWithValue("@SlNo", intSlNo);
                            sqlCmd.Parameters.AddWithValue("@OrderNo",txtOrderNo.Text.Trim());
                            sqlCmd.Parameters.AddWithValue("@FirstName",txtFirstName.Text.Trim());
                            sqlCmd.Parameters.AddWithValue("@LastName",txtLastName.Text.Trim());
                            sqlCmd.Parameters.AddWithValue("@IsRegular",chkIsRegular.Checked ? "true" : "false");
                            sqlCmd.Parameters.AddWithValue("@TypeId", Convert.ToInt32(cmbVehicleType.SelectedValue));
                            sqlCmd.Parameters.AddWithValue("@DateOfOrder",dtpDateOfOrder.Value);
                            sqlCmd.Parameters.AddWithValue("@Status",rbtnNew.Checked ? "New" : "Reconditioned");
                            if (isDefaultImage)
                            {
                                sqlCmd.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                            }
                            else if (intSlNo > 0 && strPreviousImage != "")
                            {
                                sqlCmd.Parameters.AddWithValue("@ImagePath", strPreviousImage);
                            }
                            else
                            {
                                sqlCmd.Parameters.AddWithValue("@ImagePath", SaveImage(ofd.FileName));
                            }
                            slNo = Convert.ToInt32(sqlCmd.ExecuteScalar());
                            tran.Commit();

                        }


                        catch { tran.Rollback(); }
                    }

                }
                using (SqlConnection sqlCon = new SqlConnection(conStr))
                {
                    sqlCon.Open();
                    using (SqlTransaction tran = sqlCon.BeginTransaction())
                    {
                        try { 
                        
                            foreach (DataGridViewRow dgvRow in dgvDetails.Rows)
                            {
                                if (dgvRow.IsNewRow) break;
                                else
                                {
                                    SqlCommand sqlCmd = new SqlCommand("OrderDetailAddOrEdit", sqlCon,tran);
                                    sqlCmd.CommandType = CommandType.StoredProcedure;
                                    sqlCmd.Parameters.AddWithValue("@DetailId", Convert.ToInt32(dgvRow.Cells["DetailId"].Value == DBNull.Value ? "0" : dgvRow.Cells["DetailId"].Value));
                                    sqlCmd.Parameters.AddWithValue("@SlNo ",slNo);
                                    sqlCmd.Parameters.AddWithValue("@Model", dgvRow.Cells["Model"].Value == DBNull.Value ? "" : dgvRow.Cells["Model"].Value);
                                    sqlCmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(dgvRow.Cells["Price"].Value == DBNull.Value ? "0" : dgvRow.Cells["Price"].Value));

                                    sqlCmd.ExecuteNonQuery();
                                   


                                }
                            }
                            tran.Commit();

                        }
                        catch { tran.Rollback(); }

                    }



                }
                LoadOrderListGrid();
                ClearAll();
                MessageBox.Show("Submitted successfuuly");
            }

        }

        private void dgvOrderList_DoubleClick(object sender, EventArgs e)
        {
            if (dgvOrderList.CurrentRow.Index != -1)
            {
                DataGridViewRow dgvRow = dgvOrderList.CurrentRow;
                intSlNo = Convert.ToInt32(dgvRow.Cells[0].Value);
                using (SqlConnection sqlCon = new SqlConnection(conStr))
                {
                    sqlCon.Open();
                    SqlDataAdapter da = new SqlDataAdapter("ViewOrderById", sqlCon);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.AddWithValue("@SlNo", intSlNo);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    DataRow dr = ds.Tables[0].Rows[0];
                    txtOrderNo.Text = dr["OrderNo"].ToString();
                    txtFirstName.Text = dr["FirstName"].ToString();
                    txtLastName.Text = dr["LastName"].ToString();
                    cmbVehicleType.SelectedValue = dr["TypeId"].ToString();
                    dtpDateOfOrder.Value = Convert.ToDateTime(dr["DateOfOrder"].ToString());
                    if (dr["Status"].ToString() == "New")
                        rbtnNew.Checked = true;
                    else rbtnNew.Checked = false;
                    if (dr["Status"].ToString() == "Reconditioned")
                        rbtnReconditioned.Checked = true;
                    else rbtnReconditioned.Checked = false;
                    if (dr["IsRegular"].ToString() == "true")
                        chkIsRegular.Checked = true;
                    else chkIsRegular.Checked = false;
                    try
                    {
                        imgImage.Image = new Bitmap(Application.StartupPath + "\\Images\\" + dr["ImagePath"].ToString());
                        strPreviousImage = dr["ImagePath"].ToString();
                        isDefaultImage = false;
                    }
                    catch
                    {
                        imgImage.Image = new Bitmap(Application.StartupPath + "\\Images\\noimage.png");
                        isDefaultImage = true;
                    }
                    
                    dgvDetails.AutoGenerateColumns = false;
                    dgvDetails.DataSource = ds.Tables[1];
                    btnDelete.Enabled = true;
                    btnSave.Text = "Update";
                    tabControl1.SelectedIndex = 0;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete the record?", "OrderRecord", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                
                using (SqlConnection sqlCon = new SqlConnection(conStr))
                {
                    sqlCon.Open();
                    SqlDataAdapter da = new SqlDataAdapter("ViewOrderById", sqlCon);
                    da.SelectCommand.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand.Parameters.AddWithValue("@SlNo",intSlNo);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    DataRow dr = ds.Tables[0].Rows[0];
                   
                    using (SqlTransaction tran = sqlCon.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand("OrderInfoDelete", sqlCon, tran);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@SlNo",intSlNo);
                            da.Dispose();
                            cmd.ExecuteNonQuery();
                            tran.Commit();

                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }

                    LoadOrderListGrid();
                    ClearAll();
                    MessageBox.Show("Order deleted successfully");
                }

            }
        }

        private void dgvDetails_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DataGridViewRow dgvRow =dgvDetails.CurrentRow;
            if (dgvRow.Cells["DetailId"].Value != DBNull.Value)
            {
                if (MessageBox.Show("Are you sure to delete the record?", "Details", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    using (SqlConnection sqlCon = new SqlConnection(conStr))
                    {
                        sqlCon.Open();
                        SqlCommand cmd = new SqlCommand("DetailsDelete",sqlCon);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DetailId", Convert.ToInt32(dgvRow.Cells["DetailId"].Value));
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            using (SqlConnection sqlCon = new SqlConnection(conStr))
            {
                SqlCommand cmd = sqlCon.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select o.SlNo,o.OrderNo,o.FirstName,o.LastName,o.DateOfOrder,o.ImagePath,o.IsRegular,o.Status,o.TypeId,d.Model,d.Price,(select Type From VehicleType t where t.TypeId=o.TypeId ) as Type from OrderInfo o join Details d on o.SlNo=d.SlNo";
                sqlCon.Open();
                DataTable dt = new DataTable();
                var rdr = cmd.ExecuteReader();
                dt.Load(rdr, LoadOption.Upsert);
                List<VehicleViewModel> list = new List<VehicleViewModel>();
                VehicleViewModel obj;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    obj = new VehicleViewModel();
                    obj.SlNo = Convert.ToInt32(dt.Rows[i]["SlNo"].ToString());
                    obj.FirstName = dt.Rows[i]["FirstName"].ToString();
                    obj.LastName = dt.Rows[i]["LastName"].ToString();
                    obj.OrderNo = dt.Rows[i]["OrderNo"].ToString();
                    obj.Status = dt.Rows[i]["Status"].ToString();
                    obj.TypeId = Convert.ToInt32(dt.Rows[i]["TypeId"].ToString());
                    obj.Type = dt.Rows[i]["Type"].ToString();
                    obj.Model = dt.Rows[i]["Model"].ToString();
                    obj.DateOfOrder = Convert.ToDateTime(dt.Rows[i]["DateOfOrder"].ToString());
                    obj.IsRegular = Convert.ToBoolean(dt.Rows[i]["IsRegular"].ToString());
                    obj.Price = Convert.ToDecimal(dt.Rows[i]["Price"].ToString());
                    obj.ImagePath = Application.StartupPath + "\\Images\\" + dt.Rows[i]["ImagePath"].ToString();
                    list.Add(obj);

                }
                using (ReportForm frm = new ReportForm(list))
                {
                    frm.ShowDialog();

                }

            }
        }
    }
}
