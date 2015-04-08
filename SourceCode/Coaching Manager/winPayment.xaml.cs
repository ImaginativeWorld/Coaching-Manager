﻿/// <aboutDev>
/// 
/// Project:
///     Coaching Manager (Coaching Mangement System)
/// 
/// Documentation:
///     Md. Mahmudul Hasan Shohag
///     Founder, CEO of Imaginative World
///     http://shohag.imaginativeworld.org
///     
/// Lisence:
///     Opensource project lisense under MPL 2.0.
///     Copyright © Imaginative World. All rights researved.
///     http://imaginativeworld.org
/// 
/// **************************************************
///     This Source Code Form is subject to the
///     terms of the Mozilla Public License, v.
///     2.0. If a copy of the MPL was not
///     distributed with this file, You can obtain
///     one at http://mozilla.org/MPL/2.0/.
/// **************************************************
/// 
/// </aboutDev>

using System;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coaching_Manager
{
    /// <summary>
    /// Interaction logic for winPayment.xaml
    /// </summary>
    public partial class winPayment : Window
    {

        Boolean ValidID = false;

        public winPayment()
        {
            InitializeComponent();
            SetValues();
        }

        private void SetValues()
        {
            //for getting last month
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);

            lblWinTitle.Content = Title + " | " + Strings.AppName + " | " + Strings.InstituteName;
            txtYear.Text = DateTime.Today.Year.ToString();
            cmbBxMonth.SelectedIndex = first.Month - 1;
            dPTransaction.SelectedDate = DateTime.Today;

            grpBxPayCalc.Visibility = Visibility.Hidden;

            lblInstituteName.Content = Strings.InstituteName;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // To Show a window we need to write below two line
            MainWindow win = new MainWindow();
            win.Show();
            // this line for close this form.
            this.Close();
        }

        private void gMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBxType.SelectedIndex != -1)
            {
                if (Convert.ToInt32((txtAmount.Text == "") ? "0" : txtAmount.Text) != 0)
                {
                    if (MessageBox.Show(Strings.str_transaction_confirmation, Strings.str_attention, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        if (ValidID)
                        {
                            if (cmbBxType.SelectedIndex == 0) // Student
                                AddStudentPayment();
                            else if (cmbBxType.SelectedIndex == 1) // Teacher
                                AddTeacherPayment();
                        }
                        else if (cmbBxType.SelectedIndex == 2) // Cost
                            AddCost();
                        else
                            cmTools.showInfoMsg(Strings.str_invalid_id);
                    }
                }
                else
                    cmTools.showInfoMsg(Strings.str_zero_tk_payment);
            }
            else
                cmTools.showInfoMsg(Strings.str_select_what_type);

        }

        private void AddStudentPayment()
        {
            try
            {
                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(@"INSERT INTO TblFee
                        ([ID], [Year], [Month], [Date], [Fee])
                        VALUES (@ID, @Year, @Month, @Date, @Fee)", connection);

                int intMonth = cmbBxMonth.SelectedIndex + 1;

                command.Parameters.AddWithValue("@ID", txtID.Text); // Datatype: Integer (2 bytes)
                command.Parameters.AddWithValue("@Year", txtYear.Text); // Datatype: Integer (2 bytes)
                command.Parameters.AddWithValue("@Month", intMonth); // Datatype: Byte [Only number from 0 to 255]
                command.Parameters.AddWithValue("@Date", dPTransaction.SelectedDate); // Datatype: Date/Time
                command.Parameters.AddWithValue("@Fee", txtAmount.Text); // Datatype: Integer (2 bytes)

                if ((txtID.Text == "") || (txtYear.Text == "") || (txtAmount.Text == ""))
                {
                    MessageBox.Show(Strings.strFillupAllFields);
                }
                else
                {

                    connection.Open();

                    command.ExecuteNonQuery();

                    MessageBox.Show(Strings.str_fee_added);

                    // Release Memory
                    connection.Close();
                    command.Dispose();
                    connection.Dispose();

                    ViewReceipt(txtID.Text,
                        lblName.Content.ToString(),
                        txtYear.Text,
                        cmbBxMonth.Text,
                        txtAmount.Text,
                        Convert.ToDateTime(dPTransaction.SelectedDate).ToLongDateString(),
                        true);
                }

                // Release Memory
                command.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void AddTeacherPayment()
        {
            try
            {
                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(@"INSERT INTO TblTeacherPayment
                        ([ID], [Year], [Month], [Date], [Amount])
                        VALUES (@ID, @Year, @Month, @Date, @Amount)", connection);

                int intMonth = cmbBxMonth.SelectedIndex + 1;

                command.Parameters.AddWithValue("@ID", txtID.Text); // Datatype: Integer (2 bytes)
                command.Parameters.AddWithValue("@Year", txtYear.Text); // Datatype: Integer (2 bytes)
                command.Parameters.AddWithValue("@Month", intMonth); // Datatype: Byte [Only number from 0 to 255]
                command.Parameters.AddWithValue("@Date", dPTransaction.SelectedDate); // Datatype: Date/Time
                command.Parameters.AddWithValue("@Amount", txtAmount.Text); // Datatype: Integer (2 bytes)

                if ((txtID.Text == "") || (txtYear.Text == "") || (txtAmount.Text == ""))
                {
                    MessageBox.Show(Strings.strFillupAllFields);
                }
                else
                {

                    connection.Open();

                    command.ExecuteNonQuery();
                    MessageBox.Show(Strings.str_payment_added);

                    // Release Memory
                    connection.Close();
                    command.Dispose();
                    connection.Dispose();

                    ViewReceipt(txtID.Text,
                        lblName.Content.ToString(),
                        txtYear.Text,
                        cmbBxMonth.Text,
                        txtAmount.Text,
                        Convert.ToDateTime(dPTransaction.SelectedDate).ToLongDateString(),
                        false);
                }

                // Release Memory
                command.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddCost()
        {
            try
            {
                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(@"INSERT INTO TblCost
                        ([Title], [Cost], [Date])
                        VALUES (@Title, @Cost, @Date)", connection);

                int intMonth = cmbBxMonth.SelectedIndex + 1;

                command.Parameters.AddWithValue("@Title", txtYear.Text); // Datatype:  Max 255 Char
                command.Parameters.AddWithValue("@Cost", txtAmount.Text); // Datatype: Integer Long
                command.Parameters.AddWithValue("@Date", dPTransaction.SelectedDate); // Datatype: Date/Time

                if ((txtYear.Text == "") || (txtAmount.Text == ""))
                {
                    MessageBox.Show(Strings.strFillupAllFields);
                }
                else
                {

                    connection.Open();

                    command.ExecuteNonQuery();
                    MessageBox.Show(Strings.str_cost_added);

                    // Release Memory
                    connection.Close();
                    command.Dispose();
                    connection.Dispose();

                    cmTools.AddLog(string.Format(Strings.str_log_cost_details, txtAmount.Text, txtYear.Text), this.Title);

                    // We don't need a receipt for adding cost.. ;)
                    //ViewReceipt(txtID.Text,
                    //    lblName.Content.ToString(),
                    //    txtYear.Text,
                    //    cmbBxMonth.Text,
                    //    txtAmount.Text,
                    //    Convert.ToDateTime(dPTransaction.SelectedDate).ToLongDateString(),
                    //    false);
                }

                // Release Memory
                command.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Close Window
            {
                // To Show a window we need to write below two line
                MainWindow win = new MainWindow();
                win.Show();
                // this line for close this form.
                this.Close();
            }

        }

        private void ViewReceipt(string strID, string strName, string strYear, string strMonth, string strAmount, string DT, Boolean IsStudent)
        {
            int count = 0;
            try
            {
                string strQuery = (IsStudent) ? "SELECT [ReceiptNo] FROM TblFee" : "SELECT [ReceiptNo] FROM TblTeacherPayment";

                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(strQuery, connection);

                connection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    count++;
                }

                // Release Memory
                connection.Close();
                command.Dispose();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (IsStudent)
                cmTools.AddLog(string.Format(Strings.str_log_student_transaction_details, count.ToString("D6"), strAmount, strName, strID), this.Title);
            else
                cmTools.AddLog(string.Format(Strings.str_log_teacher_transaction_details, count.ToString("D6"), strAmount, strName, strID), this.Title);


            lblTxtName.Content = (IsStudent) ? "Student Name:" : "Teacher Name:";

            lblReceiptID.Content = strID;
            lblReceiptName.Content = strName;
            lblReceiptYear.Content = strYear;
            lblReciptMonth.Content = strMonth;
            lblReceiptAmount.Content = strAmount + Strings.str_currency_end_sign;
            lblReceiptDate.Content = DT;
            lblReceiptNo.Content = count.ToString("D6");

            gridPayReceipt.Visibility = Visibility.Visible;
        }

        private void SearchStudentName()
        {
            string name = "", fee = "";
            try
            {
                string queryString =
                    "SELECT [Name], [MonthlyFee] from TblStudent WHERE [ID] = @srcID";

                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(queryString, connection);

                command.Parameters.AddWithValue("@srcID", txtID.Text);

                //Console.WriteLine(command.CommandText.ToString());

                connection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    name = reader[0].ToString();
                    fee = reader[1].ToString();
                }

                reader.Close();
                connection.Close();
                // Release Memory
                command.Dispose();
                connection.Dispose();

                if (name == "")
                {
                    lblName.Content = string.Format(Strings.str_no_object_found_with_this_id, "Student");
                    ValidID = false;
                }
                else
                {
                    lblName.Content = name;
                    txtAmount.Text = fee;
                    ValidID = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SearchTeacherName()
        {
            string name = "", PayScale = "";
            try
            {
                string queryString =
                    "SELECT [Name], [PayScale] from TblTeacher WHERE [ID] = @srcID";

                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(queryString, connection);

                command.Parameters.AddWithValue("@srcID", txtID.Text);

                //Console.WriteLine(command.CommandText.ToString());

                connection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    name = reader[0].ToString();
                    PayScale = reader[1].ToString();
                }

                reader.Close();
                connection.Close();
                // Release Memory
                command.Dispose();
                connection.Dispose();

                if (name == "")
                {
                    lblName.Content = string.Format(Strings.str_no_object_found_with_this_id, "Teacher");
                    ValidID = false;
                }
                else
                {
                    lblName.Content = name;
                    txtFeePerClass.Text = PayScale;
                    ValidID = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cmbBxType.SelectedIndex != -1)
            {
                int x;
                if (int.TryParse(txtID.Text, out x))
                    if (cmbBxType.SelectedIndex == 0)
                        SearchStudentName();
                    else
                        SearchTeacherName();
                else
                {
                    lblName.Content = Strings.str_invalid_id;
                    ValidID = false;
                }
            }
        }

        private void cmbBxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((txtID.IsEnabled == false) && (cmbBxType.SelectedIndex != -1))
            {
                txtID.IsEnabled = true;
                btnSearchName.IsEnabled = true;
                txtYear.IsEnabled = true;
                cmbBxMonth.IsEnabled = true;
                txtAmount.IsEnabled = true;
                dPTransaction.IsEnabled = true;
            }

            if (cmbBxType.SelectedIndex == 0) //Student
            {
                grpBxPayCalc.Visibility = Visibility.Hidden;
                lblYear.Content = Strings.str_lbl_year_title;
                txtYear.Text = DateTime.Today.Year.ToString(); ;

                btnSearchName.Visibility = Visibility.Visible;
                lblName.Visibility = Visibility.Visible;
                lblID.Visibility = Visibility.Visible;
                txtID.Visibility = Visibility.Visible;
                lblMonth.Visibility = Visibility.Visible;
                cmbBxMonth.Visibility = Visibility.Visible;
            }
            else if (cmbBxType.SelectedIndex == 1) //Teacher
            {
                grpBxPayCalc.Visibility = Visibility.Visible;
                lblYear.Content = Strings.str_lbl_year_title;
                txtYear.Text = DateTime.Today.Year.ToString(); ;

                btnSearchName.Visibility = Visibility.Visible;
                lblName.Visibility = Visibility.Visible;
                lblID.Visibility = Visibility.Visible;
                txtID.Visibility = Visibility.Visible;
                lblMonth.Visibility = Visibility.Visible;
                cmbBxMonth.Visibility = Visibility.Visible;
            }
            else if (cmbBxType.SelectedIndex == 2) //Cost
            {
                grpBxPayCalc.Visibility = Visibility.Hidden;
                lblYear.Content = Strings.str_lbl_cost_title;
                txtYear.Text = "";

                btnSearchName.Visibility = Visibility.Hidden;
                lblName.Visibility = Visibility.Hidden;
                lblID.Visibility = Visibility.Hidden;
                txtID.Visibility = Visibility.Hidden;
                lblMonth.Visibility = Visibility.Hidden;
                cmbBxMonth.Visibility = Visibility.Hidden;
            }

            resetAll();
        }

        private void resetAll()
        {
            txtID.Text = "";
            lblName.Content = "";
            txtAmount.Text = "";
            txtTotalClass.Text = "0";
            txtFeePerClass.Text = "0";
            txtTotalAmount.Text = "";
        }

        private void txtTotalClass_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!((cmbBxType.SelectedIndex == 2) && ((sender as TextBox).Name == "txtYear")))
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }
        }

        private void txtTotalClass_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if ((txtTotalClass.Text != "") && (txtFeePerClass.Text != ""))
                    txtTotalAmount.Text = (Convert.ToInt32(txtTotalClass.Text) * Convert.ToInt32(txtFeePerClass.Text)).ToString();
            }
            catch (Exception) { }
        }

        private void txtTotalAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtAmount.Text = txtTotalAmount.Text;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                //dlg.PrintVisual(panel.Content as Visual, "Print Button");

                printDialog.PrintVisual(borderReceiptView, Strings.str_print_payment_receipt_title);

                cmTools.AddLog(Strings.str_log_payment_receipt_printed, this.Title);
            }

        }

        private void btnCloseReceipt_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Strings.str_close_pay_reciept_confirmation, Strings.str_attention, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //// To Show a window we need to write below two line
                MainWindow win = new MainWindow();
                win.Show();
                // this line for close this form.
                this.Close();
            }
        }

        private void btnCornerMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void txtYear_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!((cmbBxType.SelectedIndex == 2) && ((sender as TextBox).Name == "txtYear")))
            {
                if (!((Convert.ToDecimal(txtYear.Text) > 2000) && (Convert.ToDecimal(txtYear.Text) < 2099)))
                {
                    int year = DateTime.Now.Year;
                    txtYear.Text = year.ToString();
                    cmTools.showInfoMsg(string.Format(Strings.str_invalid_year_changed_to_default_year, year));
                }
            }
        }
    }
}
