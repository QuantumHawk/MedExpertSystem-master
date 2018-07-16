using MedExpertSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MedExpertSystem.Database
{
    public class TibetanTestData
    {
        public List<TibetanQuestionsModel> TAQuestionsBase { get; set; }

        public TibetanTestData()
        {
            string strConnString = ConfigurationManager.ConnectionStrings["entityFramework"].ConnectionString;
            if (TAQuestionsBase == null)
            {
               TAQuestionsBase = new List<TibetanQuestionsModel>();
               DataTable tblTAQuestions = new DataTable("DataList");
                //Number, id, disease,val
                tblTAQuestions.Columns.Add("ID", typeof(int));
                tblTAQuestions.Columns.Add("Observations", typeof(string));
                tblTAQuestions.Columns.Add("Wind", typeof(string));
                tblTAQuestions.Columns.Add("Bile", typeof(string));
                tblTAQuestions.Columns.Add("Phlegm", typeof(string));
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = "SELECT ID,Observations,Wind,Bile,Phlegm FROM TibetanQustions";
                        try
                        {
                            if (con.State != ConnectionState.Open)
                                con.Open();
                            cmd.Connection = con;
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(tblTAQuestions);
                            }
                        }
                        catch (Exception expt)
                        {
                            throw new Exception(expt.Message);
                        }
                    }
                }
                foreach (DataRow r in tblTAQuestions.Rows)
                {

                        TAQuestionsBase.Add(new TibetanQuestionsModel
                    {
                        Index = r.Field<int>("ID") - 1,
                        TibetanQuestionAnswerOptionOne = r.Field<string>("Observations"),
                        TibetanAnswerOne = r.Field<string>("Wind"),
                        TibetanAnswerTwo = r.Field<string>("Bile"),
                        TibetanAnswerThree = r.Field<string>("Phlegm")
                    });
                }
            }
            
        }
    }
}