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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OleDb;

namespace Coaching_Manager
{
    /// <summary>
    /// Interaction logic for winAddExamResult.xaml
    /// </summary>
    public partial class winAddExamResult : Window
    {
        Boolean IsAdded = false;

        public winAddExamResult()
        {
            InitializeComponent();
            SetValues();
        }

        private void SetValues()
        {
            lblWinTitle.Content = Title + " | " + Strings.InstituteName;
            dpDateOfXm.SelectedDate = DateTime.Today.Date;
        }

        public class listItem
        {
            public string CoachingRoll { get; set; }
            public string StudentName { get; set; }
            public string StudentAchievement { get; set; }
        }

        private void gMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (IsAdded)
                cmTools.AddLog("Exam Result Added.", this.Title);

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

        private void cmbBxClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbBxClass.SelectedIndex != -1)
                searchNow();
        }

        private void searchNow()
        {
            try
            {
                string queryString =
                    "SELECT [ID], [Name] from TblStudent WHERE [Class] = @srcClass AND [IsActive] = @srcIsActive";

                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(queryString, connection);


                command.Parameters.AddWithValue("@srcClass", cmbBxClass.SelectedIndex.ToString());

                command.Parameters.AddWithValue("@srcIsActive", true);

                connection.Open();

                OleDbDataReader reader = command.ExecuteReader();

                this.lstView.Items.Clear();

                while (reader.Read())
                {
                    this.lstView.Items.Add(new listItem
                    {
                        CoachingRoll = reader[0].ToString(),
                        StudentName = reader[1].ToString()
                    });
                }

                reader.Close();
                connection.Close();
                // Release Memory
                command.Dispose();
                connection.Dispose();

                if (this.lstView.Items.Count != 0)
                    lstView.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lstView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtID.Text = "";
            lblName.Content = "...";
            try
            {
                var selectedlistItem = lstView.SelectedItems[0] as listItem;
                if (selectedlistItem == null)
                {
                    return;
                }

                txtID.Text = selectedlistItem.CoachingRoll;
                lblName.Content = selectedlistItem.StudentName;

                if (string.IsNullOrEmpty(selectedlistItem.StudentAchievement))
                    btnAdd.Content = " Add";
                else
                    btnAdd.Content = " Update";
            }
            catch (Exception) { }
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if ((lstView.Items.Count != 0) && (lstView.SelectedIndex != 0))
                lstView.SelectedIndex = lstView.SelectedIndex - 1;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if ((lstView.Items.Count != 0) && (lstView.SelectedIndex != lstView.Items.Count - 1))
                lstView.SelectedIndex = lstView.SelectedIndex + 1;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selectedlistItem = lstView.SelectedItem as listItem;

            if (lblName.Content.ToString() != "...")
                if (string.IsNullOrEmpty(selectedlistItem.StudentAchievement))
                    AddExamResult();
                else
                    UpdateExamResult();
        }

        private void AddExamResult()
        {
            {
                try
                {
                    OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                    OleDbCommand command = new OleDbCommand(@"INSERT INTO TblExam 
            ([ID], [Date], [Subjects], [TotalMarks], [Achievement])
            VALUES (@ID, @Date, @Subjects, @TotalMarks, @Achievement)", connection);

                    command.Parameters.AddWithValue("@ID", txtID.Text); // Datatype: Long Integer
                    command.Parameters.AddWithValue("@Date", dpDateOfXm.SelectedDate); // Datatype: Datatype: Date/Time
                    command.Parameters.AddWithValue("@Subjects", txtSub.Text); // Datatype: Max 255 Char
                    command.Parameters.AddWithValue("@TotalMarks", txtTotalMarks.Text); // Datatype: Integer (2 bytes)
                    command.Parameters.AddWithValue("@Achievement", txtAchievement.Text); // Datatype: Integer (2 bytes)

                    if ((txtID.Text == "") || (txtSub.Text == "") || (txtTotalMarks.Text == "")
                        || (txtAchievement.Text == ""))
                    {
                        cmTools.showInfoMsg("Fill up all required fields.");
                    }
                    else
                    {

                        connection.Open();

                        command.ExecuteNonQuery();

                        //Update Present Field
                        var selectedlistItem = lstView.SelectedItem as listItem;
                        selectedlistItem.StudentAchievement = txtAchievement.Text;
                        lstView.Items.Refresh();

                        // Go to next row
                        if (chkGoNextStdnt.IsChecked == true)
                            if ((lstView.Items.Count != 0) && (lstView.SelectedIndex != lstView.Items.Count - 1))
                                lstView.SelectedIndex = lstView.SelectedIndex + 1;
                            else
                                cmTools.showInfoMsg("No more student found!");

                        IsAdded = true;

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
            }
        }

        private void UpdateExamResult()
        {
            try
            {
                OleDbConnection connection = new OleDbConnection(Strings.DBconStr);
                OleDbCommand command = new OleDbCommand(@"UPDATE TblExam 
SET [Subjects] = @strSubjects, [TotalMarks] = @strTotalMarks,  [Achievement] = @strAchievement
WHERE [ID] = @strID AND [Date] = @strDate", connection);

                command.Parameters.AddWithValue("@strID", txtID.Text); // Datatype: Long Integer
                command.Parameters.AddWithValue("@strDate", dpDateOfXm.SelectedDate); // Datatype: Datatype: Date/Time
                command.Parameters.AddWithValue("@strSubjects", txtSub.Text); // Datatype: Max 255 Char
                command.Parameters.AddWithValue("@strTotalMarks", txtTotalMarks.Text); // Datatype: Integer (2 bytes)
                command.Parameters.AddWithValue("@strAchievement", txtAchievement.Text); // Datatype: Integer (2 bytes)

                connection.Open();

                command.ExecuteNonQuery();

                // Release Memory
                connection.Close();
                command.Dispose();
                connection.Dispose();

                //Update Present Field
                var selectedlistItem = lstView.SelectedItem as listItem;
                selectedlistItem.StudentAchievement = txtAchievement.Text;
                lstView.Items.Refresh();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtAchievement_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtAchievement.Text != "")
                if (Convert.ToInt32(txtAchievement.Text) > Convert.ToInt32(txtTotalMarks.Text))
                {
                    cmTools.showInfoMsg("Achievement is greter then Total Marks!");
                    txtAchievement.Text = txtTotalMarks.Text;
                }
        }

        private void btnCornerMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
