using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace Automated_Delivery_Planning_System
{
    public static class CVRP
    {
        //public const int n = 12;
        //public const int k = 5;

        public struct Client
        {
            public int number;
            public string address;  // адрес
            public int q;           // количество груза           

            public Client(string[] args)
            {
                number = Convert.ToInt32(args[0]);
                address = args[1];
                q = Convert.ToInt32(args[2]);
            }

            public void Show_Client()
            {
                Form1.textBox1.Text += Environment.NewLine + number.ToString() + " Адрес: " + address.ToString() + " Количество груза: " + q.ToString();
                //Console.WriteLine("\n{0} Адрес: {1} Количество груза: {2} ", number, address, q);
            }
        }
        /*public struct Stock
        {
            public string address;  // адрес

            public Stock(string s)
            {
                address = s;
            }

            public void Show_Stock()
            {
                Console.WriteLine("\nАдрес: {0} ", address);
            }
        }*/

        public struct Avto
        {
            public string number;
            public int b;

            public Avto(string[] args)
            {
                number = args[0];
                b = Convert.ToInt32(args[1]);
            }

            public void Show_Avto()
            {
                Form1.textBox1.Text += Environment.NewLine + "Гос. номер:" + number.ToString() + " Грузоподъёмность: " + b.ToString();
               // Console.WriteLine("\nГос. номер: {0} \nГрузоподъёмность: {1} ", number, b);
            }
        }
        public static void SettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
                MessageBox.Show("Внимание: " + e.Message);
            else if (e.Severity == XmlSeverityType.Error)
                MessageBox.Show("Ошибка записи: " + e.Message);
        }

        public static void ReadXML_Clients(string file1)
        {
            XmlReaderSettings clientSettings = new XmlReaderSettings();
            clientSettings.Schemas.Add(null, "Clients.xsd");
            clientSettings.ValidationType = ValidationType.Schema;
            XmlReader xReader = XmlReader.Create(file1, clientSettings);
            XmlDocument document = new XmlDocument();
            try
            {
                ValidationEventHandler eventHandler = new
                    ValidationEventHandler(SettingsValidationEventHandler);
                document.Load(xReader);
                document.Validate(eventHandler);
                xReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
                xReader.Close();
                return;
            }

            string[] contr1 = new string[3];
            
            XmlNodeList xClient = document.DocumentElement.ChildNodes;
            Form1.v = new Client[xClient.Count];
            int i = 0;

            foreach (XmlNode xCl in xClient)
            {
                contr1[0] = Convert.ToString(xCl.SelectSingleNode("Number").InnerText);
                contr1[1] = Convert.ToString(xCl.SelectSingleNode("Address").InnerText);
                contr1[2] = Convert.ToString(xCl.SelectSingleNode("Cargo").InnerText);
                Form1.v[i] = new Client(contr1);
                i++;
            }
        }

        public static void ReadXML_Cars(string file1)
        {
            XmlReaderSettings carSettings = new XmlReaderSettings();
            carSettings.Schemas.Add(null, "Cars.xsd");
            carSettings.ValidationType = ValidationType.Schema;
            XmlReader xReader = XmlReader.Create(file1, carSettings);
            XmlDocument document = new XmlDocument();
            try
            {
                ValidationEventHandler eventHandler = new
                    ValidationEventHandler(SettingsValidationEventHandler);
                document.Load(xReader);
                document.Validate(eventHandler);
                xReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
                xReader.Close();
                return;
            }

            string[] contr1 = new string[2];

            XmlNodeList xCar = document.DocumentElement.ChildNodes;
            Form1.avto = new Avto[xCar.Count];
            int i = 0;

            foreach (XmlNode xCl in xCar)
            {
                contr1[0] = Convert.ToString(xCl.SelectSingleNode("Number").InnerText);
                contr1[1] = Convert.ToString(xCl.SelectSingleNode("Carrying").InnerText);
                Form1.avto[i] = new Avto(contr1);
                i++;
            }
        }

        /*public static void Reading_File(Client[] v, Avto[] avto)
        {
            try
            {   // откройте текстовый файл с помощью программы чтения потока.
                using (StreamReader sr = new StreamReader("CustomerAddresses.txt", Encoding.GetEncoding(1251)))
                {
                    int i = 1, j = 0;
                    v[0] = new Client(sr.ReadLine().Split('|'));
                    if (sr.ReadLine() == "Клиенты:")
                        while (i < n + 1)
                        {
                            v[i] = new Client(sr.ReadLine().Split('|'));
                            i++;
                        }
                    if (sr.ReadLine() == "Машины:")
                        while (!sr.EndOfStream)
                        {
                            avto[j] = new Avto(sr.ReadLine().Split('|'));
                            j++;
                        }
                    //вывод прочитанного из файла
                    //foreach (Client l in v)
                    //    l.Show_Client();
                    //foreach (Avto l in avto)
                    //    l.Show_Avto();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("не удалось прочитать файл:");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }*/

        public static void Get_Of_Distance_Matrix(double[][] c) //изменить заполнение
        {
            /*c[0][0] = 0; c[0][1] = 7; c[0][2] = 4; c[0][3] = 12.4; c[0][4] = 5.1; c[0][5] = 12; c[0][6] = 7.3; c[0][7] = 6.1; c[0][8] = 14.8; c[0][9] = 7.3; c[0][10] = 5; c[0][11] = 9.2; c[0][12] = 7.3;
            c[1][0] = 0; c[1][1] = 0; c[1][2] = 11; c[1][3] = 12.6; c[1][4] = 9.4; c[1][5] = 8.2; c[1][6] = 11.4; c[1][7] = 13; c[1][8] = 13; c[1][9] = 8.6; c[1][10] = 11.4; c[1][11] = 2.8; c[1][12] = 8.6;
            c[2][0] = 0; c[2][1] = 0; c[2][2] = 0; c[2][3] = 13.9; c[2][4] = 5.8; c[2][5] = 15.3; c[2][6] = 7.3; c[2][7] = 2.2; c[2][8] = 17; c[2][9] = 9.2; c[2][10] = 3; c[2][11] = 13.2; c[2][12] = 9.2;
            c[3][0] = 0; c[3][1] = 0; c[3][2] = 0; c[3][3] = 0; c[3][4] = 17.5; c[3][5] = 7.2; c[3][6] = 7.1; c[3][7] = 14.2; c[3][8] = 4.1; c[3][9] = 19; c[3][10] = 11.4; c[3][11] = 15.2; c[3][12] = 5.1;
            c[4][0] = 0; c[4][1] = 0; c[4][2] = 0; c[4][3] = 0; c[4][4] = 0; c[4][5] = 16.4; c[4][6] = 12; c[4][7] = 7.8; c[4][8] = 19.7; c[4][9] = 3.6; c[4][10] = 8.5; c[4][11] = 10.4; c[4][12] = 12.4;
            c[5][0] = 0; c[5][1] = 0; c[5][2] = 0; c[5][3] = 0; c[5][4] = 0; c[5][5] = 0; c[5][6] = 11; c[5][7] = 16.6; c[5][8] = 5.4; c[5][9] = 16.6; c[5][10] = 13.9; c[5][11] = 10; c[5][12] = 7.1;
            c[6][0] = 0; c[6][1] = 0; c[6][2] = 0; c[6][3] = 0; c[6][4] = 0; c[6][5] = 0; c[6][6] = 0; c[6][7] = 7.2; c[6][8] = 10.8; c[6][9] = 14.6; c[6][10] = 4.5; c[6][11] = 14.2; c[6][12] = 7;
            c[7][0] = 0; c[7][1] = 0; c[7][2] = 0; c[7][3] = 0; c[7][4] = 0; c[7][5] = 0; c[7][6] = 0; c[7][7] = 0; c[7][8] = 17.7; c[7][9] = 11.3; c[7][10] = 2.8; c[7][11] = 15.3; c[7][12] = 10;
            c[8][0] = 0; c[8][1] = 0; c[8][2] = 0; c[8][3] = 0; c[8][4] = 0; c[8][5] = 0; c[8][6] = 0; c[8][7] = 0; c[8][8] = 0; c[8][9] = 20.6; c[8][10] = 14.9; c[8][11] = 15.1; c[8][12] = 7.8;
            c[9][0] = 0; c[9][1] = 0; c[9][2] = 0; c[9][3] = 0; c[9][4] = 0; c[9][5] = 0; c[9][6] = 0; c[9][7] = 0; c[9][8] = 0; c[9][9] = 0; c[9][10] = 11.7; c[9][11] = 8.6; c[9][12] = 14;
            c[10][0] = 0; c[10][1] = 0; c[10][2] = 0; c[10][3] = 0; c[10][4] = 0; c[10][5] = 0; c[10][6] = 0; c[10][7] = 0; c[10][8] = 0; c[10][9] = 0; c[10][10] = 0; c[10][11] = 13.9; c[10][12] = 7.2;
            c[11][0] = 0; c[11][1] = 0; c[11][2] = 0; c[11][3] = 0; c[11][4] = 0; c[11][5] = 0; c[11][6] = 0; c[11][7] = 0; c[11][8] = 0; c[11][9] = 0; c[11][10] = 0; c[11][11] = 0; c[11][12] = 11.4;
            c[12][0] = 0; c[12][1] = 0; c[12][2] = 0; c[12][3] = 0; c[12][4] = 0; c[12][5] = 0; c[12][6] = 0; c[12][7] = 0; c[12][8] = 0; c[12][9] = 0; c[12][10] = 0; c[12][11] = 0; c[12][12] = 0;
            */
            c[0][1] = 7; c[0][2] = 4; c[0][3] = 12.4; c[0][4] = 5.1; c[0][5] = 12; c[0][6] = 7.3; c[0][7] = 6.1; c[0][8] = 14.8; c[0][9] = 7.3; c[0][10] = 5; c[0][11] = 9.2; c[0][12] = 7.3;
            c[1][2] = 11; c[1][3] = 12.6; c[1][4] = 9.4; c[1][5] = 8.2; c[1][6] = 11.4; c[1][7] = 13; c[1][8] = 13; c[1][9] = 8.6; c[1][10] = 11.4; c[1][11] = 2.8; c[1][12] = 8.6;
            c[2][3] = 13.9; c[2][4] = 5.8; c[2][5] = 15.3; c[2][6] = 7.3; c[2][7] = 2.2; c[2][8] = 17; c[2][9] = 9.2; c[2][10] = 3; c[2][11] = 13.2; c[2][12] = 9.2;
            c[3][4] = 17.5; c[3][5] = 7.2; c[3][6] = 7.1; c[3][7] = 14.2; c[3][8] = 4.1; c[3][9] = 19; c[3][10] = 11.4; c[3][11] = 15.2; c[3][12] = 5.1;
            c[4][5] = 16.4; c[4][6] = 12; c[4][7] = 7.8; c[4][8] = 19.7; c[4][9] = 3.6; c[4][10] = 8.5; c[4][11] = 10.4; c[4][12] = 12.4;
            c[5][6] = 11; c[5][7] = 16.6; c[5][8] = 5.4; c[5][9] = 16.6; c[5][10] = 13.9; c[5][11] = 10; c[5][12] = 7.1;
            c[6][7] = 7.2; c[6][8] = 10.8; c[6][9] = 14.6; c[6][10] = 4.5; c[6][11] = 14.2; c[6][12] = 7;
            c[7][8] = 17.7; c[7][9] = 11.3; c[7][10] = 2.8; c[7][11] = 15.3; c[7][12] = 10;
            c[8][9] = 20.6; c[8][10] = 14.9; c[8][11] = 15.1; c[8][12] = 7.8;
            c[9][10] = 11.7; c[9][11] = 8.6; c[9][12] = 14;
            c[10][11] = 13.9; c[10][12] = 7.2;
            c[11][12] = 11.4;

            for (int i = 0; i < Form1.v.Length; i++)
            {
                int j = 0;
                while (j < i + 1)
                {
                    c[i][j] = 0;
                    j++;
                }
            }
        }

        public static void Get_Of_Kilometer_Wage_Matrix(double[][] s, double[][] c)
        {
            for (int i = 0; i < Form1.v.Length; i++)
                for (int j = 0; j < Form1.v.Length; j++)
                    if (i != j)
                        s[i][j] = c[0][i] + c[0][j] - c[i][j];
                    else s[i][j] = 0;

            for (int i = 0; i < Form1.v.Length; i++)
            {
                int j = 0;
                while (j < i + 1)
                {
                    s[i][j] = 0;
                    j++;
                }
            }
        }

        public static int[] Search_For_The_Maximum_Kilometer_Winnings(int[][] s_max_matr, double[][] s, Client[][] routes, Client[] v)
        {
            Search_For_Another_Cell:
            bool condition1 = true, condition2 = true, condition3 = false;
            double s_max = 0;
            int iz = 0, jz = 0;

            int[] number_v = new int[v.Length];
            for (int i = 0; i < v.Length; i++)
                number_v[i] = v[i].number;
            bool condition_i = false, condition_j = false;

            for (int i = 0; i < Form1.v.Length; i++)
                for (int j = 0; j < Form1.v.Length; j++)
                {
                    int k = 0;
                    while (k < v.Length)
                    {
                        if (i == v[k].number)
                            condition_i = true;
                        if (j == v[k].number)
                            condition_j = true;
                        if (condition_i & condition_j)
                            if ((s[i][j] > s_max) & (s_max_matr[i][j] != 1)) //условие 3
                            {
                                s_max = s[i][j];
                                iz = i;
                                jz = j;
                                condition3 = true;
                            }
                        k++;
                    }
                }

            if ((iz == 0) & (jz == 0))
            {
                int[] coordinates_null = new int[4];
                coordinates_null[0] = 0;
                coordinates_null[1] = 0;
                coordinates_null[2] = 0;
                coordinates_null[3] = 0;
                return coordinates_null;
            }
            s_max_matr[iz][jz] = 1;
            s_max_matr[jz][iz] = 1;

            int ir1 = 0, jr1 = 0, ir2 = 0, jr2 = 0;
            int iz1 = 0, jz1 = 0;
            for (int i = 0; i < v.Length; i++)
            {
                if (v[i].number == iz)
                    iz1 = i;

                if (v[i].number == jz)
                    jz1 = i;
            }

            for (int i = 0; i < routes.Length; i++)
                for (int j = 0; j < routes[i].Length; j++)
                {
                    if (routes[i][j].Equals(v[iz1]))
                    {
                        ir1 = i;
                        jr1 = j;
                    }
                    if (routes[i][j].Equals(v[jz1]))
                    {
                        ir2 = i;
                        jr2 = j;
                    }
                }

            //условие 1
            Client[] joint_route = new Client[routes[ir1].Length + routes[ir2].Length - 3];
            joint_route = routes[ir1].Union(routes[ir2]).ToArray();

            int g = 0;
            while ((condition1 == true) & (g < routes.Length))
            {
                Client[] joint_route2 = routes[g].Intersect(joint_route).ToArray();
                if ((joint_route2.Equals(joint_route)) || (ir1 == ir2))
                    condition1 = false;
                g++;
            }

            //условие 2
            if (((jr1 != 1) & (jr2 != 1)) || ((jr1 != routes[ir1].Length - 2) & (jr2 != routes[ir2].Length - 2)) || ((jr1 != 1) & (jr2 != routes[ir2].Length - 2)) || ((jr2 != 1) & (jr1 != routes[ir1].Length - 2)))
                condition2 = false;

            int[] coordinates = new int[4];
            if (condition1 & condition2 & condition3)
            {
                coordinates[0] = ir1;
                coordinates[1] = jr1;
                coordinates[2] = ir2;
                coordinates[3] = jr2;
                return coordinates;
            }
            else goto Search_For_Another_Cell;
        }

        public static Client[][] Join_Routes(Client[][] routes, int[] coordinates, int q, Client[][] built_routes)//
        {
            int ir1 = coordinates[0];
            int jr1 = coordinates[1];
            int ir2 = coordinates[2];
            int jr2 = coordinates[3];

            Client[] route1 = new Client[routes[ir1].Length];
            route1 = routes[ir1];
            Client[] route2 = new Client[routes[ir2].Length];
            route2 = routes[ir2];

            int q1 = 0, q2 = 0;
            for (int i = 0; i < route1.Length; i++)
                q1 += route1[i].q;
            for (int i = 0; i < route2.Length; i++)
                q2 += route2[i].q;

            Client[] route3 = new Client[route1.Length + route2.Length - 2];
            bool reverse = false, correct_amount = true;
            if (q1 + q2 <= q)
            {
                if ((((jr1 == 1) & (jr2 == 1)) || ((jr1 == route1.Length - 2) & (jr2 == route2.Length - 2))))
                {
                    if ((route1.Length == 3) & (jr2 == route2.Length - 2))
                        Array.Reverse(route2);
                    else
                    {
                        if ((route2.Length == 3) & (jr1 != route1.Length - 2))
                            Array.Reverse(route1);
                    }
                    reverse = true;
                }

                if (reverse)
                {
                    for (int i = 0; i < route1.Length - 1; i++)
                        route3[i] = route1[i];

                    for (int i = 0; i < route2.Length - 1; i++)
                        route3[route1.Length - 1 + i] = route2[i + 1];
                }
                else
                {
                    if ((jr1 == 1) & (jr2 == route2.Length - 2))
                    {
                        for (int i = 0; i < route2.Length - 1; i++)
                            route3[i] = route2[i];

                        for (int i = 0; i < route1.Length - 1; i++)
                            route3[route2.Length - 1 + i] = route1[i + 1];
                    }
                    else
                    {
                        for (int i = 0; i < route1.Length - 1; i++)
                            route3[i] = route1[i];

                        for (int i = 0; i < route2.Length - 1; i++)
                            route3[route1.Length - 1 + i] = route2[i + 1];
                    }
                }
            }
            else correct_amount = false;

            if (correct_amount)
            {
                //удаление лишних маршрутов после объединения
                if (routes.Length == 0) return routes;// Проверки, что наш массив не пуст 
                if (routes.Length <= ir2) return routes;//и что указанный индекс существует.

                built_routes = new Client[routes.Length - 1][];
                int counter = 0;

                for (int i = 0; i < routes.Length; i++)
                {
                    if (i == ir2) continue;
                    if (i == ir1)
                    {
                        built_routes[counter] = new Client[route3.Length];
                        built_routes[counter] = route3;
                    }
                    else
                        built_routes[counter] = routes[i];
                    counter++;
                }
            }
            else
            {
                built_routes = new Client[routes.Length][];
                for (int i = 0; i < routes.Length; i++)
                    built_routes[i] = routes[i];
            }
            return built_routes;
        }

        public static Client[][] BubbleSort_Routes(Client[][] routes)
        {
            for (int i = 0; i < routes.Length - 1; i++)
                for (int j = i + 1; j < routes.Length; j++)
                {
                    int all_q_1 = 0, all_q_2 = 0;
                    foreach (Client l in routes[i])
                        all_q_1 += l.q;
                    foreach (Client l in routes[j])
                        all_q_2 += l.q;

                    if (all_q_1 < all_q_2)
                    {
                        Client[] temp = new Client[routes[i].Length];
                        temp = routes[i];
                        routes[i] = new Client[routes[j].Length];
                        routes[i] = routes[j];
                        routes[j] = new Client[temp.Length];
                        routes[j] = temp;
                    }
                }
            return routes;
        }

        public static Avto[] BubbleSort_Avto(Avto[] avto)
        {
            for (int i = 0; i < avto.Length - 1; i++)
                for (int j = i + 1; j < avto.Length; j++)
                    if (avto[i].b < avto[j].b)
                    {
                        Avto temp = new Avto();
                        temp = avto[i];
                        avto[i] = avto[j];
                        avto[j] = temp;
                    }
            return avto;
        }

        public static Client[][] Add_Finished_Routes(Client[][] routes, Client[][] ready_made_routes, int q_count_cars)
        {
            Client[][] selected_routes2 = new Client[ready_made_routes.Length + q_count_cars][];
            int counter = ready_made_routes.Length;
            int j = 0;
            for (int i = 0; i < selected_routes2.Length; i++)
                if (i < counter)
                {
                    selected_routes2[i] = new Client[ready_made_routes.Length];
                    selected_routes2[i] = ready_made_routes[i];
                }
                else
                {
                    if ((q_count_cars != j) & (routes.Length != j))
                    {
                        selected_routes2[i] = new Client[routes.Length];
                        selected_routes2[i] = routes[j];
                        j++;
                    }
                }
            return selected_routes2;
        }

        public static Client[] Getting_Unused_Vertices(Client[] v_rest, Client[][] routes, int q_count_cars)
        {
            int a = 0;
            for (int i = 0; i < q_count_cars; i++)
                a = routes[i].Length - 2;

            Client[] v_cop2 = new Client[v_rest.Length];
            v_cop2 = v_rest;
            Client[] v_cop = new Client[v_rest.Length - a];
            for (int i = 0; i < q_count_cars; i++)
            {
                int k1 = 0;
                while (k1 < v_cop2.Length)
                {
                    for (int j = 1; j < routes[i].Length - 1; j++)
                    {
                        if ((routes[i][j].Equals(v_cop2[k1])) & (v_cop2[k1].number != 0))
                        {
                            //удаление вершин которые уже добавлены в готовые маршруты
                            if (v_cop.Length == 0) return v_cop;// Проверки, что наш массив не пуст 
                            v_cop = new Client[v_cop2.Length - 1];
                            int counter = 0;

                            for (int i1 = 0; i1 < v_cop2.Length; i1++)
                            {
                                if (i1 == k1) continue;
                                v_cop[counter] = v_cop2[i1];
                                counter++;
                            }
                            v_cop2 = new Client[v_cop.Length];
                            v_cop2 = v_cop;
                        }
                    }
                    k1++;
                }
            }
            return v_cop;
        }

        public static Client[][] Permutation_Generation(Client[] m_rout, int count, Client[][] mass_p_rout, Client[] temp, int m)
        {
            for (int i = 0; i < m_rout.Length; i++)
            {
                temp[count] = m_rout[i];
                if (count == (m - 1))
                {
                    int kk = 0;
                    for (int h = 0; h < Form1.mass_p_rout.Length; h++)
                        if (mass_p_rout[h] != null)
                            kk++;

                    Form1.mass_p_rout[kk] = new Client[temp.Length];
                    for (int j = 0; j < temp.Length; j++)
                        Form1.mass_p_rout[kk][j] = temp[j];

                    return Form1.mass_p_rout;
                }
                else
                {
                    Client[] m_rout2 = new Client[m_rout.Length - 1];
                    int counter1 = 0;

                    for (int i1 = 0; i1 < m_rout.Length; i1++)
                    {
                        if (i1 == i) continue;
                        m_rout2[counter1] = m_rout[i1];
                        counter1++;
                    }
                    int count2 = count + 1;
                    Form1.mass_p_rout = Permutation_Generation(m_rout2, count2, Form1.mass_p_rout, temp, m);
                }
            }
            return Form1.mass_p_rout;
        }

        public static Client[][] Route_Optimization(Client[][] ready_made_routes, Client[][] mass_p_rout, double[][] c, int i)
        {
            double[] s_routes = new double[mass_p_rout.Length];
            for (int i1 = 0; i1 < mass_p_rout.Length; i1++)
            {
                int w = 0;
                foreach (Client l in mass_p_rout[i1])
                    w += l.q;

                int cc1 = 0;
                int cc2 = mass_p_rout[i1][0].number;

                s_routes[i1] = s_routes[i1] + c[cc1][cc2] * (w - mass_p_rout[i1][0].q);

                for (int j = 1; j < mass_p_rout[i1].Length; j++)
                {
                    int c1 = mass_p_rout[i1][j - 1].number;
                    int c2 = mass_p_rout[i1][j].number;

                    s_routes[i1] = s_routes[i1] + c[c1][c2] * (w - mass_p_rout[i1][j].q);
                }
            }

            double min_s_routes = s_routes[0];
            int min_i = 0;
            for (int g1 = 0; g1 < s_routes.Length; g1++)
                if (s_routes[g1] < min_s_routes)
                {
                    min_s_routes = s_routes[g1];
                    min_i = g1;
                }

            for (int j = 1; j < ready_made_routes[i].Length - 1; j++)
                ready_made_routes[i][j] = mass_p_rout[min_i][j - 1];

            return ready_made_routes;
        }
    }
}
