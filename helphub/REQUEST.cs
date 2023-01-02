﻿using FluentValidation;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static helphub.COMPLAINT;
using static helphub.REGISTER;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace helphub
{
    public partial class REQUEST : Form
    {

        public class Request
        {
            public string DREQUEST { get; set; }
            public string Address { get; set; }
            public string Contact { get; set; }
            public string City { get; set; }
        }

        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(Request => Request.DREQUEST).NotNull().WithMessage("Kindly Provide Proper Details about Request");
                RuleFor(Request => Request.Address).NotNull();
                RuleFor(Request => Request.Contact).NotNull().Matches("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$");
                RuleFor(Complaint => Complaint.City).NotNull();
            }
        }
        public REQUEST()
        {
            InitializeComponent();
            Aadhar.Text = UserData.aadharno;
            Contact.Text = UserData.mobilenumber;
            ComboBox1.SelectedItem = "MEDICAL EMERGENCY";
            comboBox2.SelectedItem = "AP|Andhra Pradesh";
            ComboBox1.Select();
            Address.Text = UserData.address;
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            LOGIN login = new LOGIN();

            login.Show();

            this.Hide(); //Close Form1,the current open form.
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            Request request = new Request();
            RequestValidator validator = new RequestValidator();

            request.Address = Address.Text;
            request.Contact = Contact.Text;
            request.DREQUEST = DREQUEST.Text;
            request.City = city.Text;


            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                String errors = "Kindly Solve Below Errors\n";
                int i = 1;
                foreach (var failure in result.Errors)
                {
                    errors = "" + errors + " " + i + ") Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage + "\n";
                    i++;
                }
                MessageBox.Show(errors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DREQUEST.Text.Trim() == "" && Address.Text.Trim() == "" && Aadhar.Text.Trim() == "" && Contact.Text.Trim() == "" && ComboBox1.SelectedItem.ToString() == "")
            {
                MessageBox.Show("Empty Fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    string SQLitecnStr = @"Data Source=./helphub.db";
                    SQLiteConnection SQLiteConn = new SQLiteConnection();
                    SQLiteCommand SQLitecmd = new SQLiteCommand();

                    SQLiteConn.ConnectionString = SQLitecnStr;
                    SQLiteConn.Open();
                    SQLitecmd.Connection = SQLiteConn;
                    String typeofcomplain = ComboBox1.SelectedItem.ToString();
                    String state = comboBox2.SelectedItem.ToString();

                    SQLitecmd.CommandText = "insert into request(aadharno,typeofrequest,mobilenumber,aboutrequest,address,state,city,username) VALUES('" + Aadhar.Text + "','" + typeofcomplain + "','" + Contact.Text + "','" + DREQUEST.Text + "','" + Address.Text + "','"+ state + "','" + city.Text + "','" + UserData.username + "')";

                    try
                    {

                        SQLitecmd.ExecuteNonQuery();
                        MessageBox.Show("Request received, Checkout in status section", "Request", MessageBoxButtons.OK);
                        CreateLogs.userlogobj.userlog(UserData.username, "User sent a request", this.Name);
                        STATUS status = new STATUS();

                        status.Show();

                        this.Hide(); //Close Form1,the current open form.
                    }
                    catch (Exception ex)
                    {
                        CreateLogs.userlogobj.userlog(UserData.username, "Can't Register Your Request: " + ex.Message , this.Name);
                        MessageBox.Show("Can't Register Your Request: " + ex.Message + "", "Request", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    SQLiteConn.Close();
                }
                catch (Exception ex)
                {
                    CreateLogs.userlogobj.userlog(UserData.username, "Can't Register Your Request: " + ex.Message, this.Name);
                    MessageBox.Show("Unable to Record your Request: "+ ex.Message +"", "Request", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(ex);
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            DASHBOARD dashboard = new DASHBOARD();

            dashboard.Show();

            this.Hide(); //Close Form1,the current open form.
        }
    }
}
