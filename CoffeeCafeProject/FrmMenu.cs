using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMenu : Form
    {

        // สร้างตัวแปรเก็บรูปที่แปลงเป็น byte array ลง DB
        byte[] menuImage;

        public FrmMenu()
        {
            InitializeComponent();
        }

        private Image convertByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null || byteArrayIn.Length == 0)
            {
                return null;
            }
            try
            {
                using (MemoryStream ms = new MemoryStream(byteArrayIn))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (ArgumentException ex)
            {
                // อาจเกิดขึ้นถ้า byte array ไม่ใช่ข้อมูลรูปภาพที่ถูกต้อง
                Console.WriteLine("Error converting byte array to image: " + ex.Message);
                return null;
            }
        }

        private byte[] convertImageToByteArray(Image image, ImageFormat imageFormat)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, imageFormat);
                return ms.ToArray();
            }
        }

        private void getAllMenuToListView()
        {
            // Connect String เพื่อเชื่อมต่อฐานข้อมูล ตามยี่ห้อของฐานข้อมูลที่ใช้
            string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";
            // Create connection object ไปยังฐานข้อมูลที่ต้องการ
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล

                    // SELECT, INSERT, UPDATE, DELETE
                    // สร้างคำสั่ง SQL เพื่อดึงข้อมูลจากตาราง product_tb
                    string strSQL = "SELECT menuId, menuName, menuPrice, menuImage FROM menu_tb";

                    // สร้าง SqlCommand เพื่อรันคำสั่ง SQL
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        // สร้าง DataTable แปลงจากเป็นก้อนมาเป็นตาราง
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // ตั้งค่าทั่วไปของ ListView
                        lvShowAllMenu.Items.Clear(); // ล้างข้อมูลเก่าใน ListView
                        lvShowAllMenu.Columns.Clear(); // ล้างคอลัมน์เก่าใน ListView
                        lvShowAllMenu.FullRowSelect = true; // เลือกแถวทั้งหมดเมื่อคลิกที่แถวใดแถวหนึ่ง
                        lvShowAllMenu.View = View.Details; // ตั้งค่าให้แสดงผลแบบรายละเอียด

                        // ตั้งค่าการแสดงรูปใน ListView
                        if (lvShowAllMenu.SmallImageList == null)
                        {
                            lvShowAllMenu.SmallImageList = new ImageList();
                            lvShowAllMenu.SmallImageList.ImageSize = new Size(40, 40); // กำหนดขนาดของรูปภาพ
                            lvShowAllMenu.SmallImageList.ColorDepth = ColorDepth.Depth32Bit; // กำหนดความลึกของสี
                        }
                        lvShowAllMenu.SmallImageList.Images.Clear(); // ล้างรูปภาพเก่าใน ImageList

                        // กำหนดรายละเอียดของ Column ใน ListView
                        lvShowAllMenu.Columns.Add("รูปเมนู", 80, HorizontalAlignment.Left); // เพิ่มคอลัมน์ใหม่
                        lvShowAllMenu.Columns.Add("รหัสเมนู", 60, HorizontalAlignment.Left); // เพิ่มคอลัมน์ใหม่
                        lvShowAllMenu.Columns.Add("ชื่อเมนู", 150, HorizontalAlignment.Left); // เพิ่มคอลัมน์ใหม่
                        lvShowAllMenu.Columns.Add("ราคาเมนู", 80, HorizontalAlignment.Left); // เพิ่มคอลัมน์ใหม่

                        // LOOP เพื่อเพิ่มข้อมูลจาก DataTable ลงใน ListView
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            ListViewItem item = new ListViewItem(); // สร้าง item เก็บข้อมูลแต่ละรายการ

                            Image menuImage = null; // ตัวแปรสำหรับเก็บรูปภาพ
                            if (dataRow["menuImage"] != DBNull.Value)
                            {
                                byte[] imgByte = (byte[])dataRow["menuImage"];
                                // แปลงข้อมูลรูปภาพจากฐานข้อมูลเป็น byte array
                                menuImage = convertByteArrayToImage(imgByte); // แปลง byte array เป็น Image
                            }

                            string imagekey = null;// ตัวแปรสำหรับเก็บ key ของรูปภาพ
                            if (menuImage != null)
                            {
                                imagekey = $"menu_{dataRow["menuId"]}"; // สร้าง key สำหรับรูปภาพ
                                lvShowAllMenu.SmallImageList.Images.Add(imagekey, menuImage); // เพิ่มรูปภาพลงใน ImageList
                                item.ImageKey = imagekey; // กำหนด key ของรูปภาพให้กับ item
                            }
                            else
                            {
                                item.ImageIndex = -1;
                            }
                            //เพิ่มรายการลงใน item ตามข้อมูลใน DataRow

                            item.SubItems.Add(dataRow["menuId"].ToString());
                            item.SubItems.Add(dataRow["menuName"].ToString());
                            item.SubItems.Add(dataRow["menuPrice"].ToString());

                            // เพิ่ม item ลงใน ListView
                            lvShowAllMenu.Items.Add(item);


                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
                }
            }
        }

        private void FrmMenu_Load(object sender, EventArgs e)
        {
            getAllMenuToListView();
            menuImage = null;
            pbMenuImage.Image = null;
            tbMenuId.Clear();
            tbMenuName.Clear();
            tbMenuPrice.Clear();
            btSave.Enabled = true;
            btUpdate.Enabled = false;
            btDelete.Enabled = false;
        }

        private void btSelectMenuImage_Click(object sender, EventArgs e)
        {
            // open file dialog เพื่อเลือกไฟล์รูปภาพ jpg, png
            // ถ้าเลือกไฟล์ได้ ให้แสดงรูปภาพใน pcbProImage
            // แปลงเป็น byte array เก็บไว้ในตัวแปรเพื่อใช้ในการบันทึกฐานข้อมูล
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\\"; // กำหนดโฟลเดอร์เริ่มต้น Drive C
            openFileDialog.Filter = "Image Files|*.jpg;*.png;";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // แสดงรูปภาพใน PictureBox
                pbMenuImage.Image = Image.FromFile(openFileDialog.FileName);

                // ตรวจสอบ Formant ของรูปภาพ แล้วแปลงเป็น byte array
                if (pbMenuImage.Image.RawFormat == ImageFormat.Jpeg)
                {
                    menuImage = convertImageToByteArray(pbMenuImage.Image, ImageFormat.Jpeg);
                }
                else
                {
                    menuImage = convertImageToByteArray(pbMenuImage.Image, ImageFormat.Png);
                }
            }

        }
        private void showWarningMessage(string message)
        {
            MessageBox.Show(message, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btSave_Click(object sender, EventArgs e)
        {

            // Validate UI Inputs (WarningDialog) แล้วเอาข้อมูลบันทึกลงฐานข้อมูล
            // เสร็จแล้วปิดหน้าจอ FrmProductCreate และกลับไปที่ FrmProductShow
            if (menuImage == null)
            {
                showWarningMessage("กรุณาเลือกรูปภาพเมนู");
            }
            else if (tbMenuName.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกชื่อเมนู");
            }
            else if (tbMenuPrice.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกราคาเมนู");
            }
            else
            {
                // บันทึกลง DB 
                // สร้าง connection string ไปยังฐานข้อมูลที่ต้องการ
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";

                // Create connection object ไปยังฐานข้อมูลที่ต้องการ
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล

                        // ถ้าเกิน 10 รายการ ให้แสดงข้อความเตือน
                        String countSQL = "SELECT COUNT(*) FROM menu_tb";
                        using (SqlCommand conutCommand = new SqlCommand(countSQL, sqlConnection))
                        {
                            int rowCount = (int)conutCommand.ExecuteScalar(); // นับจำนวนแถวในตาราง menu_tb
                            if (rowCount == 10)
                            {
                                showWarningMessage("มีเมนูได้แค่ 10 รายการเท่านั้น กรุณาลบรายการที่ไม่ต้องการออกก่อน");
                                return; // return คือ ออกจาก Method นี้ทันที
                            }
                        }

                        // For Insert, Update, Delete
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

                        // สร้างคำสั่ง SQL สำหรับการเพิ่มข้อมูลสินค้าใหม่
                        string strSQL = "INSERT INTO menu_tb (menuName, menuPrice, menuImage) " +
                                        "VALUES (@menuName, @menuPrice, @menuImage)";

                        // กำหนดค่าให้กับ Sql Parameters และสั่งให้คำสั่ง SQL ทำงาน
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@menuName", SqlDbType.NVarChar, 100).Value = tbMenuName.Text;
                            sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(tbMenuPrice.Text);
                            sqlCommand.Parameters.Add("@menuImage", SqlDbType.Image).Value = menuImage;

                            // รันคำสั่ง SQL
                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            MessageBox.Show(Text = "บันทึกข้อมูลสินค้าเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Update LisView และ Clear
                            getAllMenuToListView();
                            menuImage = null; // Clear image variable
                            pbMenuImage.Image = null; // Clear PictureBox
                            tbMenuId.Clear(); // Clear Menu ID textbox
                            tbMenuName.Clear(); // Clear Menu Name textbox  
                            tbMenuPrice.Clear(); // Clear Menu Price textbox
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
                        return;
                    }
                }
            }
        }

        private void tbMenuPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, one decimal point, backspace, and delete
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

            // Prevent multiple decimal points
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains('.'))
            {
                e.Handled = true;
            }
        }

        private void lvShowAllMenu_ItemActivate(object sender, EventArgs e)
        {
            // double clock ที่ ListView เพื่อแสดงข้อมูลใน TextBox และปุ่ม Update, Delete ใฃ้ได้
            tbMenuId.Text = lvShowAllMenu.SelectedItems[0].SubItems[1].Text;
            tbMenuName.Text = lvShowAllMenu.SelectedItems[0].SubItems[2].Text;
            tbMenuPrice.Text = lvShowAllMenu.SelectedItems[0].SubItems[3].Text;

            //เอารูปภาพที่เลือกมาแสดงใน PictureBox
            var item = lvShowAllMenu.SelectedItems[0];
            if (!string.IsNullOrEmpty(item.ImageKey) && lvShowAllMenu.SmallImageList.Images.ContainsKey(item.ImageKey))
            {
                pbMenuImage.Image = lvShowAllMenu.SmallImageList.Images[item.ImageKey];
            }
            else
            {
                pbMenuImage.Image = null; // Clear PictureBox if no image is found
            }

            btSave.Enabled = false;
            btUpdate.Enabled = true;
            btDelete.Enabled = true;
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            // กดยืนยีนก่อนลบข้อมูล
            if (MessageBox.Show("ต้องการลบเมนูหรือไม่", "ยืนยัน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // ลบข้อมูลจากฐานข้อมูล
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล
                        // สร้างคำสั่ง SQL สำหรับการลบข้อมูลเมนู
                        string strSQL = "DELETE FROM menu_tb WHERE menuId = @menuId";
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@menuId", SqlDbType.Int).Value = int.Parse(tbMenuId.Text);
                            sqlCommand.ExecuteNonQuery(); // รันคำสั่ง SQL

                            MessageBox.Show("ลบเมนูเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Update ListView และ Clear
                            getAllMenuToListView();
                            menuImage = null; // Clear image variable
                            pbMenuImage.Image = null; // Clear PictureBox
                            tbMenuId.Clear(); // Clear Menu ID textbox
                            tbMenuName.Clear(); // Clear Menu Name textbox  
                            tbMenuPrice.Clear(); // Clear Menu Price textbox
                            btUpdate.Enabled = false;
                            btDelete.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
                    }
                }
            }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            // Validate UI Inputs (WarningDialog) แล้วเอาข้อมูลบันทึกลงฐานข้อมูล
            if (tbMenuName.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกชื่อเมนู");
            }
            else if (tbMenuPrice.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกราคาเมนู");
            }
            else
            {
                // บันทึกลง DB 
                // สร้าง connection string ไปยังฐานข้อมูลที่ต้องการ
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";

                // Create connection object ไปยังฐานข้อมูลที่ต้องการ
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล

                        // For Insert, Update, Delete
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

                        // สร้างคำสั่ง SQL จะมี 2 แบบคือ แก้รูปภาพหรือไม่แก้ไขรูปภาพ
                        string strSQL = "";
                        if (menuImage == null)
                        {
                            strSQL = "UPDATE menu_tb SET menuName = @menuName, menuPrice = @menuPrice " +
                                    "WHERE menuId = @menuId";
                        }
                        else
                        {
                            strSQL = "UPDATE menu_tb SET menuName = @menuName, menuPrice = @menuPrice, menuImage = @menuImage " +
                                    "WHERE menuId = @menuId";
                        }

                        // กำหนดค่าให้กับ Sql Parameters และสั่งให้คำสั่ง SQL ทำงาน
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@menuId", SqlDbType.Int).Value = int.Parse(tbMenuId.Text);
                            sqlCommand.Parameters.Add("@menuName", SqlDbType.NVarChar, 100).Value = tbMenuName.Text;
                            sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(tbMenuPrice.Text);

                            if (menuImage != null)
                            {
                                sqlCommand.Parameters.Add("@menuImage", SqlDbType.Image).Value = menuImage;
                            }
                            
                            // รันคำสั่ง SQL
                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            MessageBox.Show(Text = "แก้ไขเมนูเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Update LisView และ Clear
                            getAllMenuToListView();
                            menuImage = null; // Clear image variable
                            pbMenuImage.Image = null; // Clear PictureBox
                            tbMenuId.Clear(); // Clear Menu ID textbox
                            tbMenuName.Clear(); // Clear Menu Name textbox  
                            tbMenuPrice.Clear(); // Clear Menu Price textbox
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
                        return;
                    }
                }
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            getAllMenuToListView();
            menuImage = null;
            pbMenuImage.Image = null;
            tbMenuId.Clear();
            tbMenuName.Clear();
            tbMenuPrice.Clear();
            btSave.Enabled = true;
            btUpdate.Enabled = false;
            btDelete.Enabled = false;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close(); // ปิดหน้าจอ FrmMenu
        }
    }
}
