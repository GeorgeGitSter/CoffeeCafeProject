using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoffeeCafeProject
{
    public partial class FrmMember : Form
    {
        public FrmMember()
        {
            InitializeComponent();
        }

        private void resetForm()
        { 
            tbMemberId.Text = ""; // ล้างข้อมูลใน TextBox รหัสสมาชิก
            tbMemberPhone.Text = ""; // ล้างข้อมูลใน TextBox เบอร์โทรสมาชิก
            tbMemberName.Text = ""; // ล้างข้อมูลใน TextBox ชื่อสมาชิก
            btSave.Enabled = true;
            btUpdate.Enabled = false;
            btDelete.Enabled = false;
        }

        private void getAllMemberToListView()
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
                    string strSQL = "SELECT memberId, memberPhone, memberName FROM member_tb";

                    // สร้าง SqlCommand เพื่อรันคำสั่ง SQL
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(strSQL, sqlConnection))
                    {
                        // สร้าง DataTable แปลงจากเป็นก้อนมาเป็นตาราง
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // ตั้งค่าทั่วไปของ ListView
                        lvShowAllMember.Items.Clear(); // ล้างข้อมูลเก่าใน ListView
                        lvShowAllMember.Columns.Clear(); // ล้างคอลัมน์เก่าใน ListView
                        lvShowAllMember.FullRowSelect = true; // เลือกแถวทั้งหมดเมื่อคลิกที่แถวใดแถวหนึ่ง
                        lvShowAllMember.View = View.Details; // ตั้งค่าให้แสดงผลแบบรายละเอียด

                        // กำหนดรายละเอียดของ Column ใน ListView
                        lvShowAllMember.Columns.Add("รหัสสมาชิก", 80, HorizontalAlignment.Left); // เพิ่มคอลัมน์ใหม่
                        lvShowAllMember.Columns.Add("เบอร์โทรสมาชิก", 150, HorizontalAlignment.Left); // เพิ่มคอลัมน์ใหม่
                        lvShowAllMember.Columns.Add("ชื่อสมาชิก", 100, HorizontalAlignment.Left); // เพิ่มคอลัมน์ใหม่

                        // LOOP เพื่อเพิ่มข้อมูลจาก DataTable ลงใน ListView
                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            ListViewItem item = new ListViewItem(dataRow["memberId"].ToString()); // สร้าง item เก็บข้อมูลแต่ละรายการ
                            //item.SubItems.Add(dataRow["memberId"].ToString());
                            item.SubItems.Add(dataRow["memberPhone"].ToString());
                            item.SubItems.Add(dataRow["memberName"].ToString());

                            // เพิ่ม item ลงใน ListView
                            lvShowAllMember.Items.Add(item);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
                }
            }
        }
        private void FrmMember_Load(object sender, EventArgs e)
        {
            getAllMemberToListView(); // เรียกใช้เมธอดเพื่อดึงข้อมูลสมาชิกทั้งหมดเมื่อโหลดฟอร์ม
            btUpdate.Enabled = false; // ปิดปุ่มอัพเดทเริ่มต้น
            btDelete.Enabled = false; // ปิดปุ่มลบเริ่มต้น
        }

        private void showWarningMessage(string message)
        {
            MessageBox.Show(message, "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            if (tbMemberPhone.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกเบอร์โทรสมาชิก");
            }
            else if (tbMemberName.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกชื่อสมาชิก");
            }
            else
            {
                // Connect String เพื่อเชื่อมต่อฐานข้อมูล ตามยี่ห้อของฐานข้อมูลที่ใช้
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";
                // Create connection object ไปยังฐานข้อมูลที่ต้องการ
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล
                        // สร้างคำสั่ง SQL สำหรับ INSERT ข้อมูลสมาชิกใหม่

                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

                        string strSQL = "INSERT INTO member_tb (memberPhone, memberName) " +
                            "VALUES (@memberPhone, @memberName)";
                        // สร้าง SqlCommand เพื่อรันคำสั่ง SQL
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
                            sqlCommand.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text;

                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            MessageBox.Show(Text = "บันทึกข้อมูลสมาชิกเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            getAllMemberToListView(); // เรียกใช้เมธอดเพื่อดึงข้อมูลสมาชิกทั้งหมดอีกครั้ง
                            resetForm(); // เรียกใช้เมธอดเพื่อรีเซ็ตฟอร์ม
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
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

        private void lvShowAllMember_ItemActivate(object sender, EventArgs e)
        {
            tbMemberId.Text = lvShowAllMember.SelectedItems[0].SubItems[0].Text;
            tbMemberPhone.Text = lvShowAllMember.SelectedItems[0].SubItems[1].Text;
            tbMemberName.Text = lvShowAllMember.SelectedItems[0].SubItems[2].Text;

            btSave.Enabled = false; // ปิดปุ่มบันทึก
            btUpdate.Enabled = true; // เปิดปุ่มอัพเดท
            btDelete.Enabled = true; // เปิดปุ่มลบ
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            if (tbMemberPhone.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกเบอร์โทรสมาชิก");
            }
            else if (tbMemberName.Text.Length == 0)
            {
                showWarningMessage("กรุณากรอกชื่อสมาชิก");
            }
            else
            {
                // Connect String เพื่อเชื่อมต่อฐานข้อมูล ตามยี่ห้อของฐานข้อมูลที่ใช้
                string connectionString = @"Server=DESKTOP-9U4FO0V\SQLEXPRESS;Database=coffee_cafe_db;Trusted_Connection=True";
                // Create connection object ไปยังฐานข้อมูลที่ต้องการ
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    try
                    {
                        sqlConnection.Open(); // เปิดการเชื่อมต่อกับฐานข้อมูล
                        SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
                        // สร้างคำสั่ง SQL สำหรับ UPDATE ข้อมูลสมาชิก
                        string strSQL = "UPDATE member_tb SET memberPhone = @memberPhone, memberName = @memberName " +
                            "WHERE memberId = @memberId";
                        // สร้าง SqlCommand เพื่อรันคำสั่ง SQL
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection, sqlTransaction))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = int.Parse(tbMemberId.Text);
                            sqlCommand.Parameters.Add("@memberPhone", SqlDbType.NVarChar, 50).Value = tbMemberPhone.Text;
                            sqlCommand.Parameters.Add("@memberName", SqlDbType.NVarChar, 100).Value = tbMemberName.Text;

                            sqlCommand.ExecuteNonQuery();
                            sqlTransaction.Commit();

                            MessageBox.Show(Text = "อัพเดทข้อมูลสมาชิกเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            getAllMemberToListView(); // เรียกใช้เมธอดเพื่อดึงข้อมูลสมาชิกทั้งหมดอีกครั้ง
                            resetForm(); // เรียกใช้เมธอดเพื่อรีเซ็ตฟอร์ม
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
                    }
                }
            }
        }

        private void btDelete_Click(object sender, EventArgs e)
        {
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
                        string strSQL = "DELETE FROM member_tb WHERE memberId = @memberId";
                        using (SqlCommand sqlCommand = new SqlCommand(strSQL, sqlConnection))
                        {
                            sqlCommand.Parameters.Add("@memberId", SqlDbType.Int).Value = int.Parse(tbMemberId.Text);
                            sqlCommand.ExecuteNonQuery(); // รันคำสั่ง SQL

                            MessageBox.Show("ลบเมนูเรียบร้อยแล้ว", "ผลการทำงาน", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Update ListView และ Clear
                            
                            getAllMemberToListView(); // เรียกใช้เมธอดเพื่อดึงข้อมูลสมาชิกทั้งหมดอีกครั้ง
                            resetForm(); // เรียกใช้เมธอดเพื่อรีเซ็ตฟอร์ม
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อฐานข้อมูลได้ กรุณาลองใหม่หรือติดต่อ IT\n" + ex.Message);
                    }
                }
            }
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            resetForm(); 
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
