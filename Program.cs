using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace Examen
{
    class Program
    {
        static void Main(string[] args)
        {

            SqlConnection cartiCon = new SqlConnection(@"Data Source=TIBI-PC\SQLEXPRESS;Initial Catalog=Carti;Integrated Security=True");
            cartiCon.Open();

            SqlDataAdapter cartiAdapter = new SqlDataAdapter("Select * from carti order by codCarte", cartiCon);//creaza o imagine a datelor din tabel in memoria calculatorului
            SqlCommandBuilder cursBuilder = new SqlCommandBuilder(cartiAdapter);
            DataSet cartiSet = new DataSet();// permite folosirea tuturor operatiunilor cu data

            cartiAdapter.Fill(cartiSet, "carti");

            SqlDataAdapter autoriAdapter = new SqlDataAdapter("Select * from autori order by codAutor", cartiCon);
            autoriAdapter.Fill(cartiSet, "autori");

            SqlDataAdapter librariiAdapter = new SqlDataAdapter("Select * from librarii order by codLibrarie", cartiCon);
            librariiAdapter.Fill(cartiSet, "librarii");



//Tabelul initial de carti

            Console.WriteLine("Tabelul de carti: ");
            foreach (DataRow cRow in cartiSet.Tables["carti"].Rows)
            {
                Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", cRow["codCarte"], cRow["codAutor"], cRow["titlu"], cRow["anAparitie"]);
            }


//Adaugam informatii noi in tabelul de carti

            Console.WriteLine("Adauga in tabelul de carti o noua carte de test: ");

            DataRow cartiRow = cartiSet.Tables["carti"].NewRow();
            cartiRow["codCarte"] = 5;
            cartiRow["codAutor"] = 5;
            cartiRow["titlu"] = "Carte nou adaugata";
            cartiRow["anAparitie"] = 2018;

            cartiSet.Tables["carti"].Rows.Add(cartiRow);

            try
            {
                cartiAdapter.Update(cartiSet, "carti");

                Console.WriteLine("Tabelul de carti cu cartea nou adaugata: ");
                foreach (DataRow cRow in cartiSet.Tables["carti"].Rows)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", cRow["codCarte"], cRow["codAutor"], cRow["titlu"], cRow["anAparitie"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nu s‐a adaugat. Eroare " + ex.Message);
            }


//Stergem cartea nou adaugata
            
            DataColumn[] pk = new DataColumn[1];
            pk[0] = cartiSet.Tables["carti"].Columns["codCarte"];
            cartiSet.Tables["carti"].PrimaryKey = pk;
            DataRow caut = null;
            while (caut == null)
            {
                Console.Write("Pentru a sterge o carte din baza de date, dati codul cartii de sters: ");
                int mrc = Convert.ToInt16(Console.ReadLine());
                caut = cartiSet.Tables["carti"].Rows.Find(mrc);
            }
            try
            {
                caut.Delete();
                cartiAdapter.Update(cartiSet, "carti");
                Console.WriteLine("Dupa stergerea cartii: ");
                foreach (DataRow cRow in cartiSet.Tables["carti"].Rows)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", cRow["codCarte"], cRow["codAutor"], cRow["titlu"], cRow["anAparitie"]);
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Nu s‐a sters. Eroare " + ex.Message);
            }
            
            
//Modificam anul aparitiei

            Console.WriteLine("Modificam anul aparitie pentru Ciuma: ");


            cartiSet.Tables["carti"].Rows[0]["anAparitie"] = 1947;

            try
            {
                cartiAdapter.Update(cartiSet, "carti");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Nu s‐a modificat anul aparitie. Eroare " + ex.Message);
            }

            Console.WriteLine("Tabelul de carti cu anul aparitiei modificat: ");
            foreach (DataRow cRow in cartiSet.Tables["carti"].Rows)
            {
                Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", cRow["codCarte"], cRow["codAutor"], cRow["titlu"], cRow["anAparitie"]);
            }
        
 
//Legaturile intre tabele 

        Console.WriteLine("Carti + autori + libraria unde se gaseste cartea ");

            DataRelation relCartiAutori = cartiSet.Relations.Add("codAutor", cartiSet.Tables["autori"].Columns["codAutor"], cartiSet.Tables["carti"].Columns["codAutor"]);
            DataRelation relLibrariiCarti = cartiSet.Relations.Add("codCarte", cartiSet.Tables["librarii"].Columns["codCarte"], cartiSet.Tables["carti"].Columns["codCarte"]);
                                    
            foreach (DataRow cRow in cartiSet.Tables["carti"].Rows)
            {
                Console.Write("\t{0}\t{1}\t{2}\t{3}", cRow["codCarte"], cRow["codAutor"], cRow["titlu"], cRow["anAparitie"]);
                foreach (DataRow aRow in cRow.GetParentRows(relCartiAutori))
                {
                    Console.Write(("\t{0}\t{1}"), aRow["nume"], aRow["prenume"]);

                        foreach(DataRow lRow in cRow.GetParentRows(relLibrariiCarti))
                        {
                            Console.WriteLine(("\t{0}"), lRow["denumire"]);
                        }
                }
            }
            
            cartiCon.Close();
            Console.ReadLine();
        }
    }
}
