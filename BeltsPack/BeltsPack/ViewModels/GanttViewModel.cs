using BeltsPack.Utils;
using BeltsPack.Views.Dialogs;
using Syncfusion.Windows.Controls.Gantt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeltsPack.ViewModels
{

    public class GanttViewModel
    {
        public string Codice { get; set; }
        public string Versione { get; set; }
        public ObservableCollection<TaskDetails> TaskCollection { get; set; }
        public GanttViewModel()
        {
            TaskCollection = this.GetDataSource();
        }
        private ObservableCollection<TaskDetails> GetDataSource()
        {
            int i = 0;

            // Memorizzo i dati che mi servono
            DatabaseSQL dbSQL = DatabaseSQL.CreateDefault();
            dbSQL.OpenConnection();
            SqlDataReader reader;
            SqlCommand creaComando = dbSQL.CreateDbTotaleCommand();
            reader = creaComando.ExecuteReader();
            ObservableCollection<TaskDetails> task = new ObservableCollection<TaskDetails>();
            
            while (reader.Read())
            {
                if(reader.GetValue(reader.GetOrdinal("Stato")).ToString() == "Inviato")
                {
                    // Determino le date di inizio e fine del task
                    string DataCreazioneDocumentazione = reader.GetValue(reader.GetOrdinal("Data")).ToString();
                    string DataConsegna = reader.GetValue(reader.GetOrdinal("DataConsegnaCassa")).ToString();

                    DateTime DataInizio = Convert.ToDateTime(DataCreazioneDocumentazione);
                    DateTime DataFine = Convert.ToDateTime(DataConsegna);

                    // Genero il task solo se la documentazione è stata inviata e se la data di consegna della cassa è successiva a oggi
                    if (DataFine > DateTime.Now)
                    {

                        // Genero il task
                        task.Add(
                        new TaskDetails
                        {
                            TaskId = 1,
                            TaskName = reader.GetValue(reader.GetOrdinal("Codice")).ToString() + "_" + reader.GetValue(reader.GetOrdinal("Versione")).ToString(),

                            StartDate = DataInizio,
                            FinishDate = DataFine,

                            Duration = new TimeSpan(Convert.ToInt32((DataFine - DataInizio).TotalDays), 0, 0, 0),
                        });

                    }

                }     
                i += 1;
            }
            
           
            return task;
        }
    }
}

