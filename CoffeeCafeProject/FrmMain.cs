using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMain : Form
    {

        // var for menu prices
        float[] menuPrice = new float[10];
        int memberId = 0; // Variable to member ID

        public FrmMain()
        {
            InitializeComponent();
        }
        // Method to resetForm
        private void resetFrom()
        {
            memberId = 0; // Reset member ID
            rdMenberNo.Checked = false;
            rdMemberYes.Checked = false;
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
            lbOrderPay.Text = "0.00";
            // Reset ListView
            lvOrderMenu.Items.Clear();
            lvOrderMenu.Columns.Clear();
            lvOrderMenu.FullRowSelect = true;
            lvOrderMenu.View = View.Details;
            lvOrderMenu.Columns.Add("ชื่อเมนู", 150, HorizontalAlignment.Left);
            lvOrderMenu.Columns.Add("ราคา", 80, HorizontalAlignment.Left);

            // DataBase
            using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
            {
                try
                {
                    sqlConnection.Open();

                    string strSQL = "SELECT menuName, menuPrice, menuImage FROM menu_tb";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // สร้างตัวแปรอ้างถึง PictureBox และ Button ที่จะเอาไปแสดงผล
                        PictureBox[] pbMenuImage = { pbMenu1, pbMenu2, pbMenu3, pbMenu4, pbMenu5, pbMenu6, pbMenu7, pbMenu8, pbMenu9, pbMenu10 };
                        Button[] btMenuName = { btMenu1, btMenu2, btMenu3, btMenu4, btMenu5, btMenu6, btMenu7, btMenu8, btMenu9, btMenu10 };

                        // Clear previous images and buttons ก่อนจะแสดงผลใหม่
                        for (int i = 0; i < 10; i++)
                        {
                            pbMenuImage[i].Image = Properties.Resources.menu; // Clear previous image
                            btMenuName[i].Text = "Menu"; // Clear previous button text
                        }

                        // Loop ข้อมูลใน DataTable และแสดงผลใน PictureBox, Button และ menuPrice
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            btMenuName[i].Text = dataTable.Rows[i]["menuName"].ToString();
                            menuPrice[i] = float.Parse(dataTable.Rows[i]["menuPrice"].ToString());
                            // นำรูปภาพมาแสดงใน PictureBox

                            if (dataTable.Rows[i]["menuImage"] != DBNull.Value)
                            {
                                // แปลงข้อมูลรูปภาพจากฐานข้อมูลที่เป็น Binary เป็น Image
                                byte[] imgByte = (byte[])dataTable.Rows[i]["menuImage"];
                                using (var ms = new System.IO.MemoryStream(imgByte))
                                {
                                    pbMenuImage[i].Image = System.Drawing.Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                pbMenuImage[i].Image = Properties.Resources.menu; // ถ้าไม่มีรูปภาพให้แสดงเป็น null
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการเชื่อมต่อฐานข้อมูล: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btMenu_Click(object sender, EventArgs e)
        {
            FrmMenu frmMenu = new FrmMenu();
            frmMenu.ShowDialog();
            resetFrom();
        }

        private void btMember_Click(object sender, EventArgs e)
        {
            FrmMember frmMember = new FrmMember();
            frmMember.ShowDialog();
        }


        private void btCancel_Click(object sender, EventArgs e)
        {
            resetFrom();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            resetFrom();
        }

        private void rdMenberNo_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = false;
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
            memberId = 0;
        }

        private void rdMemberYes_CheckedChanged(object sender, EventArgs e)
        {
            tbMemberPhone.Clear();
            tbMemberPhone.Enabled = true;
            tbMemberPhone.Focus();
            tbMemberName.Text = "(ชื่อสมาชิก)";
            lbMemberScore.Text = "0";
        }

        private void tbMemberPhone_KeyUp(object sender, KeyEventArgs e)
        {
            // ตรวจสอบว่ากดปุ่มแล้วปล่อย ใช่ปุ่ม Enter หรือไม่
            // ถ้าใช่ให้ทำการค้นหาข้อมูลสมาชิกด้วยเบอร์โทรศัพท์ แล้วเอาชื่อกับคะแนนมาแสดง
            if (e.KeyCode == Keys.Enter)
            {
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open();

                        string strSQL = "SELECT memberId, memberName, memberScore FROM member_tb WHERE memberPhone = @memberPhone";

                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;

                            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand))
                            {
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable);

                                if (dataTable.Rows.Count == 1)
                                {
                                    tbMemberName.Text = dataTable.Rows[0]["memberName"].ToString();
                                    lbMemberScore.Text = dataTable.Rows[0]["memberScore"].ToString();
                                    memberId = int.Parse(dataTable.Rows[0]["memberId"].ToString());
                                }
                                else
                                {
                                    MessageBox.Show("ไม่พบข้อมูลสมาชิกที่มีเบอร์โทรศัพท์นี้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("เกิดข้อผิดพลาดในการเชื่อมต่อฐานข้อมูล: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void tbMemberPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, one decimal point, backspace, and delete
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }

            if (char.IsDigit(e.KeyChar) && tbMemberPhone.Text.Length >= 10)
            {
                e.Handled = true;
            }
        }

        private void btMenu1_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu1.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu1.Text);
                item.SubItems.Add(menuPrice[0].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[0]).ToString();
            }
        }

        private void btMenu2_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu2.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu2.Text);
                item.SubItems.Add(menuPrice[1].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[1]).ToString();
            }
        }

        private void btMenu3_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu3.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu3.Text);
                item.SubItems.Add(menuPrice[2].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[2]).ToString();
            }
        }

        private void btMenu4_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu4.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu4.Text);
                item.SubItems.Add(menuPrice[3].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[3]).ToString();
            }
        }

        private void btMenu5_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu5.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu5.Text);
                item.SubItems.Add(menuPrice[3].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[3]).ToString();
            }
        }

        private void btMenu6_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu6.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu6.Text);
                item.SubItems.Add(menuPrice[5].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[5]).ToString();
            }
        }

        private void btMenu7_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu7.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu7.Text);
                item.SubItems.Add(menuPrice[6].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[6]).ToString();
            }
        }

        private void btMenu8_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu8.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu8.Text);
                item.SubItems.Add(menuPrice[7].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[7]).ToString();
            }
        }

        private void btMenu9_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu9.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu9.Text);
                item.SubItems.Add(menuPrice[8].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[8]).ToString();
            }
        }

        private void btMenu10_Click(object sender, EventArgs e)
        {
            // ตรวจสอบฃื่อบนปุ่มเป็นคำว่า "Menu" หรือไม่ ถ้าไม่ใช่ให้เอาฃื่อกับราคาไปแสดงใน ListView พร้อมกับคำนวณราคา และ สะสมแต้ม
            if (btMenu10.Text != "Menu")
            {
                // เอาฃื่อ ราคา ใส่ใน ListView
                ListViewItem item = new ListViewItem(btMenu10.Text);
                item.SubItems.Add(menuPrice[9].ToString());
                lvOrderMenu.Items.Add(item);

                // บวกแต้มเพิ่มให้สมาชิก
                // ต้องตรวจสอบว่ามีเป็นสมาชิกหรือไม่ ถ้าใช่ให้บวกแต้มเพิ่ม
                if (tbMemberName.Text != "(ชื่อสมาชิก)")
                {
                    lbMemberScore.Text = (int.Parse(lbMemberScore.Text) + 1).ToString();
                }

                // บวกราคาเพิ่มให้กับยอดรวม
                lbOrderPay.Text = (float.Parse(lbOrderPay.Text) + menuPrice[9]).ToString();
            }
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่า รวมเป็นเงิน มีค่าเป็น 0 หรือไม่ ถ้าใช่ให้แสดงข้อความเตือน
            if (lbOrderPay.Text == "0.00")
            {
                MessageBox.Show("เลือกเมนูที่จะสั่งด้วย!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (rdMemberYes.Checked == false && rdMenberNo.Checked == false)
            {
                MessageBox.Show("เลือกสถานะสมาชิกด้วย!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (rdMemberYes.Checked == true && tbMemberName.Text == "(ชื่อสมาชิก)")
            {
                MessageBox.Show("กรุณาค้นหาสมาชิกด้วย!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // ทำ 3 อย่าง 
                // 1. บันทึกเพิ่มลง order_tb
                // 2. บันทึกเพิ่มลง order_detail_tb
                // 3. บันทึกแก้ไขแต้มคะแนนสมาชิก member_tb กรณีเป็นสมาชิก
                using (SqlConnection sqlConnection = new SqlConnection(ShareResource.connectionString))
                {
                    try
                    {
                        sqlConnection.Open();
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

                        // บันทึกลง order_tb ---------------------------------------------------------------------------
                        string strSQL = "INSERT INTO order_tb (memberId, orderPay, createAt, updateAt) " +
                                        "VALUES (@memberId, @orderPay, @createAt, @updateAt);" +
                                        "SELECT CAST(SCOPE_IDENTITY() AS INT)";

                        // ตัวแปรเก็บ orderId
                        int orderId;

                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                            sqlCommand.Parameters.Add("@orderPay", SqlDbType.Float).Value = float.Parse(lbOrderPay.Text);
                            sqlCommand.Parameters.Add("@createAt", SqlDbType.DateTime).Value = DateTime.Now;
                            sqlCommand.Parameters.Add("@updateAt", SqlDbType.DateTime).Value = DateTime.Now;

                            orderId = (int)sqlCommand.ExecuteScalar(); // return primary key กลับมาเก็บใน orderId
                        }
                        // บันทึกลง order_detail_tb --------------------------------------------------------------------
                        foreach (ListViewItem item in lvOrderMenu.Items)
                        {
                            string strSQL2 = "INSERT INTO order_detail_tb (orderId, menuName, menuPrice) " +
                                             "VALUES (@orderId, @menuName, @menuPrice)";

                            using (SqlCommand sqlCommand = new SqlCommand(strSQL2, sqlConnection, sqlTransaction))
                            {
                                sqlCommand.Parameters.Add("@orderId", SqlDbType.Int).Value = orderId;
                                sqlCommand.Parameters.Add("@menuName", SqlDbType.NVarChar, 100).Value = item.SubItems[0].Text;
                                sqlCommand.Parameters.Add("@menuPrice", SqlDbType.Float).Value = float.Parse(item.SubItems[1].Text);

                                sqlCommand.ExecuteNonQuery(); // Execute the command to insert data
                            }
                        }

                        // แก้ไข memberScore ใน member_tb -------------------------------------------------------------
                        if (rdMemberYes.Checked == true)
                        {
                            string strSQL3 = "UPDATE member_tb SET memberScore = @memberScore WHERE memberId = @memberId";
                            using (SqlCommand sqlCommand = new SqlCommand(strSQL3, sqlConnection, sqlTransaction))
                            {
                                sqlCommand.Parameters.Add("@memberScore", SqlDbType.Int).Value = int.Parse(lbMemberScore.Text);
                                sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = memberId;
                                sqlCommand.ExecuteNonQuery(); // Execute the command to update data
                            }
                        }
                        sqlTransaction.Commit();
                        MessageBox.Show(Text = "บันทึกข้อมูลเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        resetFrom();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.StackTrace);
                    }
                }
            }
        }

        private void lvOrderMenu_ItemActivate(object sender, EventArgs e)
        {
            // Double click ที่รายการ แล้วเอาออกจาก ListView item เอาแต้มออก และยอดรวมลดลงด้วย 
        }
    }
}
