using System;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Data.Entity;
using System.Data.SqlServerCe;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;
using static System.Data.Entity.Database;

namespace MergeDatabases
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var mainDatabase = new MyDbContext("DataSource=|DataDirectory|southeast1.sdf;Max Database Size=4091"))
            {
                Console.WriteLine(mainDatabase.TachographDocuments.ToList().Count);

                using (var secondaryDatabase = new MyDbContext("DataSource=|DataDirectory|southeast2.sdf;Max Database Size=4091"))
                {

                    var gv212ToMerge = secondaryDatabase.Gv212Report.AsNoTracking().ToList();
                    var decomm = secondaryDatabase.LetterForDecommissioningDocuments.AsNoTracking().ToList();
                    var centerChecks = secondaryDatabase.QcReports.AsNoTracking().ToList();
                    var qcChecks = secondaryDatabase.QcReport6Month.AsNoTracking().ToList();
                    var tacho = secondaryDatabase.TachographDocuments.AsNoTracking().ToList();
                    var undownload = secondaryDatabase.UndownloadabilityDocuments.AsNoTracking().ToList();

                    foreach (var tachographDocument in tacho)
                    {
                        tachographDocument.SerializedData = null;
                    }

                    foreach (var letterForDecommissioningDocument in decomm)
                    {
                        letterForDecommissioningDocument.SerializedData = null;
                    }

                    foreach (var undownloadabilityDocument in undownload)
                    {
                        undownloadabilityDocument.SerializedData = null;
                    }

                    mainDatabase.Gv212Report.AddRange(gv212ToMerge);
                    mainDatabase.LetterForDecommissioningDocuments.AddRange(decomm);
                    mainDatabase.QcReports.AddRange(centerChecks);
                    mainDatabase.QcReport6Month.AddRange(qcChecks);
                    mainDatabase.TachographDocuments.AddRange(tacho);
                    mainDatabase.UndownloadabilityDocuments.AddRange(undownload);


                }

                try
                {

                mainDatabase.SaveChanges();
                    Console.WriteLine(mainDatabase.TachographDocuments.ToList().Count);

                }

                catch (DbEntityValidationException e)
                {
                    
                    throw;
                }

            }
        }
    }
}
