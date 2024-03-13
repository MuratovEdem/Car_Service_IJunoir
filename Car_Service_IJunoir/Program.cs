using System;
using System.Collections.Generic;

namespace Car_Service_IJunoir
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CarService carService = new CarService();
            carService.Work();
        }
    }

    class CarService
    {
        private List<List<Detail>> _details = new List<List<Detail>>();

        public CarService()
        {
            FillListDetails();
            Money = 10000;
        }

        public int Money { get; private set; }

        public void Work()
        {
            while (IsWork())
            {
                Client client = new Client();
                Detail faultyDetail = client.FaultyDetail;

                Console.WriteLine("Пришел новый клиент" +
                    $"\nНеисправность в его машине - {faultyDetail.Name}, стоимость ремонта - {faultyDetail.Cost}" +
                    $"\nДенег у клиента {client.Money}");

                if (client.Money >= faultyDetail.Cost)
                {
                    Console.WriteLine($"У автосервиса такие детали имеются в количестве {GetCountDetails(faultyDetail)} ед.");

                    if (GetCountDetails(faultyDetail) > 0)
                    {
                        RepairDetail(faultyDetail);
                        TakeMoney(client, faultyDetail.Cost);
                        Console.WriteLine("Починка произведена успешно");
                    }
                    else
                    {
                        Console.WriteLine("У автосервиса нет такой детали. Он выплачивает штраф в размере стоимости детали");
                        GiveMoney(client, faultyDetail.Cost);
                    }
                }
                else
                {
                    Console.WriteLine("У клиента недостаточно денег для ремонта");
                }

                Console.WriteLine("Для обслуживания следующего клиента нажмите любую клавишу");
                Console.ReadKey();
                Console.Clear();
            }

            Console.WriteLine("Автосервис заверщил свою работу");
        }


        public void GiveMoney(Client client, int money)
        {
            if (Money - money > 0)
            {
                Money -= money;
                client.TakeMoney(money);
            }
            else
            {
                Console.WriteLine("У автосервиса недостаточно средств для выплаты компенсации");
            }
        }

        public void TakeMoney(Client client, int money)
        {
            Money += money;
            client.GiveMoney(money);
        }
        private int GetCountDetails(Detail detail)
        {
            return _details[GetIndexListDetails(detail)].Count;
        }

        private void RepairDetail(Detail detail)
        {
            _details[GetIndexListDetails(detail)].RemoveAt(0);
        }

        private bool IsWork()
        {
            int counter = 0;

            foreach (List<Detail> details in _details)
            {
                if (details.Count == 0)
                    counter++;
            }

            return counter != _details.Count;
        }

        private int GetIndexListDetails(Detail detail)
        {
            int index = 0;

            for (int i = 0; i < _details.Count; i++)
            {
                if (_details[i].Count > 0)
                {
                    if (_details[i][0].Name == detail.Name)
                    {
                        index = i;
                        break;
                    }
                }
            }

            return index;
        }

        private void FillListDetails()
        {
            DetailsCreator detailsCreator = new DetailsCreator();

            int minNumberDetails = 1;
            int maxNumberDetails = 10;

            foreach (string nameDetail in detailsCreator.GetNameDetails())
            {
                List<Detail> temporary = new List<Detail>();
                int numberDetails = UserUtils.GenerateRandomNumber(minNumberDetails, maxNumberDetails);

                for (int i = 0; i < numberDetails; i++)
                    temporary.Add(detailsCreator.CreateDetail(nameDetail));

                _details.Add(temporary);
            }
        }
    }

    class Detail
    {
        public Detail(string name, int cost)
        {
            Name = name;
            Cost = cost;
        }

        public string Name { get; private set; }
        public int Cost { get; private set; }
    }

    class Client
    {
        public Client()
        {
            DetailsCreator faultyDetailsCreator = new DetailsCreator();

            FaultyDetail = faultyDetailsCreator.CreateRandomDetail();

            int maxNumberMoney = 10000;
            int minNumberMoney = 1000;

            Money = UserUtils.GenerateRandomNumber(minNumberMoney, maxNumberMoney);
        }

        public Detail FaultyDetail { get; private set; }
        public int Money { get; private set; }

        public void TakeMoney(int money)
        {
            Money += money;
        }

        public void GiveMoney(int money)
        {
            Money -= money;
        }
    }

    class DetailsCreator
    {
        private Dictionary<string, int> _details;

        public DetailsCreator()
        {
            _details = new Dictionary<string, int>()
            {
                { "Колодки", 500 },
                { "Масло", 1000 },
                { "Карбюратор", 2000 },
                { "Фильтр", 700 }
            };
        }

        public List<string> GetNameDetails()
        {
            return new List<string>(_details.Keys);
        }

        public Detail CreateDetail(string name)
        {
            return new Detail(name, _details[name]);
        }

        public Detail CreateRandomDetail()
        {
            List<string> keysList = new List<string>(_details.Keys);
            int randomNumber = UserUtils.GenerateRandomNumber(keysList.Count);

            return new Detail(keysList[randomNumber], _details[keysList[randomNumber]]);
        }
    }

    class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int maxValue)
        {
            return s_random.Next(maxValue);
        }

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            return s_random.Next(minValue, maxValue);
        }
    }
}