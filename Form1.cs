using Microsoft.SqlServer.Management.Sdk.Sfc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BackgroundWorkerExample
{
    
    public partial class Form1 : Form
    {

        int pageNumber = 0;
        int pageSize = 40;
        BackgroundWorker worker;
        static SaveFileDialog saveFile;


        public Form1()
        {
           InitializeComponent();

            worker = new BackgroundWorker();
            
            worker.DoWork += backgroundWorker1_DoWork;
            worker.ProgressChanged += backgroundWorker1_ProgressChanged;
            worker.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            worker.WorkerReportsProgress=true;
            progressBar1.Value = 0;
            progressBar1.Step = 10;
            progressBar1.Maximum = 100;
            progressBar1.Visible = false;
            label1.Visible = false;
            BtnPrevious.Enabled = false;
            buttonLastest.Enabled = false;
        }
   


        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: questa riga di codice carica i dati nella tabella 'adventureWorks2019DataSet.Customer'. È possibile spostarla o rimuoverla se necessario.
            this.customerTableAdapter.Fill(this.adventureWorks2019DataSet.Customer);
           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel Documents (*.xlsx)|*.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                progressBar1.Visible = true;
                label1.Visible = true;
                worker.RunWorkerAsync();
            }
          
        }
 

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
        try
        {      worker.ReportProgress(10);
                using (ExcelPackage pck = new ExcelPackage())
                {
                        
                     worker.ReportProgress(50);
                    foreach (DataTable dataTable in adventureWorks2019DataSet.Tables)
                    {
                        
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dataTable.TableName);
                        ws.Cells["A1"].LoadFromDataTable(dataTable, true);
                        ws.Cells.AutoFitColumns();
                        worker.ReportProgress(70);
                        using (ExcelRange objRange = ws.Cells["A1:G1"])
                        {
                            objRange.Style.Font.Bold = true;
                            objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            objRange.Style.Fill.BackgroundColor.SetColor(Color.Aquamarine);
                        }
                     }
                    worker.ReportProgress(80);
                    byte[] output = pck.GetAsByteArray();
                    try
                    {
                        FileStream fs = new FileStream(saveFile.FileName, FileMode.Create);
                        worker.ReportProgress(90);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(output, 0, output.Length); //write the encoded file
                        worker.ReportProgress(100);
                        bw.Flush();
                        bw.Close();
                        fs.Close();
                        MessageBox.Show("Save Completed.", "Costumers Results", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("Unable to save file. " + ex.Message, "Costumers Results", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                  }
                   

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message + " Stacktrace: " + ex.StackTrace);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label1.Text = e.ProgressPercentage.ToString() + "%";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            label1.Visible = false;
            MessageBox.Show("Finished!");
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            var query = from c in adventureWorks2019DataSet.Customer
                        select new
                        {
                            CustomerID = c.CustomerID,
                            //PersonID = c.PersonID,
                            //StoreID = c.StoreID,
                            TerritoryID = c.TerritoryID,
                            AccountNumber = c.AccountNumber,
                            RowGuideID = c.rowguid,
                            ModifiedDate= c.ModifiedDate
                          
                        };
            pageNumber--;
            customerBindingSource1.DataSource = query.Skip(pageSize * pageNumber).Take(pageSize).ToList();
           
            if (query.Skip(pageSize * (pageNumber-1)).Take(pageSize).Count() > 0)
               BtnPrevious.Enabled = true;
            else
                BtnPrevious.Enabled = false;
            BtnNext.Enabled = true;
            BtnUltimate.Enabled = true;
            if (pageNumber == 0)
            {
                BtnPrevious.Enabled = false;
                buttonLastest.Enabled = false;
            }
            else
            {
                BtnPrevious.Enabled = true;
                buttonLastest.Enabled = true;
            }
            label_navigator.Text = String.Format("Page {0}/{1}", pageNumber, query.Count() / pageSize);
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            var query = from c in adventureWorks2019DataSet.Customer
                        select new
                        {
                            CustomerID = c.CustomerID,
                            //PersonID = c.PersonID,
                           // StoreID = c.StoreID,
                            TerritoryID = c.TerritoryID,
                            AccountNumber = c.AccountNumber,
                            RowGuideID = c.rowguid,
                            ModifiedDate = c.ModifiedDate

                        };
             pageNumber++;
            customerBindingSource1.DataSource = query.Skip(pageSize * pageNumber).Take(pageSize).ToList();
           
            if (query.Skip(pageSize * (pageNumber + 1)).Take(pageSize).Count() > 0)
                BtnNext.Enabled = true;
            else
                BtnNext.Enabled = false;
            BtnPrevious.Enabled = true;
            buttonLastest.Enabled = true;
            if (pageNumber == query.Count() / pageSize)
            {
                BtnNext.Enabled = false;
                BtnUltimate.Enabled = false;
            }
            else
            {
                BtnNext.Enabled = true;
                BtnUltimate.Enabled = true;
            }
          
            label_navigator.Text = String.Format("Page {0}/{1}", pageNumber, query.Count() / pageSize);
        }

        private void BtnUltimate_Click(object sender, EventArgs e)
        {
            var query = from c in adventureWorks2019DataSet.Customer
                        select new
                        {
                            CustomerID = c.CustomerID,
                            //PersonID = c.PersonID,
                            // StoreID = c.StoreID,
                            TerritoryID = c.TerritoryID,
                            AccountNumber = c.AccountNumber,
                            RowGuideID = c.rowguid,
                            ModifiedDate = c.ModifiedDate

                        };
            pageNumber=query.Count()/(pageSize);
            customerBindingSource1.DataSource = query.Skip(pageSize * pageNumber).Take(pageSize).ToList();
            BtnNext.Enabled = false;
            BtnPrevious.Enabled = true;
            buttonLastest.Enabled = true;
            BtnUltimate.Enabled = !(pageNumber == query.Count() / pageSize);

            label_navigator.Text = String.Format("Page {0}/{1}", pageNumber, query.Count() / pageSize);
        }

        private void buttonLastest_Click(object sender, EventArgs e)
        {
            var query = from c in adventureWorks2019DataSet.Customer
                        select new
                        {
                            CustomerID = c.CustomerID,
                            //PersonID = c.PersonID,
                            // StoreID = c.StoreID,
                            TerritoryID = c.TerritoryID,
                            AccountNumber = c.AccountNumber,
                            RowGuideID = c.rowguid,
                            ModifiedDate = c.ModifiedDate

                        };
            pageNumber = 0;
            customerBindingSource1.DataSource = query.Skip(pageSize * pageNumber).Take(pageSize).ToList();
            BtnNext.Enabled = true;
            BtnPrevious.Enabled = false;
            buttonLastest.Enabled = !(pageNumber ==0);
            BtnUltimate.Enabled = true;
            label_navigator.Text = String.Format("Page {0}/{1}", pageNumber, query.Count() / pageSize);
        }
    }
}
