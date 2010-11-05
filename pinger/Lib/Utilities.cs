using System;
using System.Windows.Forms;

namespace uping.Lib
{
    public static class Utilities
    {
        public static void ExportToCSV(DataGridView dgv)
        {
            string strExport = "";
            //Loop through all the columns in DataGridView to Set the 
            //Column Heading
            foreach (DataGridViewColumn dc in dgv.Columns)
            {
                strExport += dc.Name + "   ";
            }
            strExport = strExport.Substring(0, strExport.Length - 3) + Environment.NewLine.ToString();
            //Loop through all the row and append the value with 3 spaces
            foreach (DataGridViewRow dr in dgv.Rows)
            {
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    if (dc.Value != null)
                    {
                        strExport += dc.Value.ToString() + "   ";
                    }
                }
                strExport += Environment.NewLine.ToString();
            }
            strExport = strExport.Substring(0, strExport.Length - 3) + Environment.NewLine.ToString();
            //Create a TextWrite object to write to file, select a file name with .csv extention
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = @"CSV Files|*.csv|All Files|*.*";
            sfd.FileName = string.Format("uping_{0}.csv", (DateTime.Parse(dgv.Rows[0].Cells[(dgv.Rows[0].Cells.Count - 1)].Value.ToString())).ToString("ddMMyyyyHHmmsss"));
            sfd.ShowDialog();
            System.IO.TextWriter tw = null;
            if (sfd.CheckFileExists && sfd.FileName.Length > 0)
            {
                tw = new System.IO.StreamWriter(sfd.FileName);
                //Write the Text to file
                tw.Write(strExport);
            }
            //Close the Textwrite
            if (tw != null)
                tw.Close();
        }
    }
}
