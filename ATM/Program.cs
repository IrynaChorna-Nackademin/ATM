using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace ATM
{
    class ATMBalance
    {
        public ATMBalance()
        {
            Reserves = new List<Reserve>
            {
                new Reserve { Quantity = 2, Nominal = 1000 },
                new Reserve { Quantity = 3, Nominal = 500 },
                new Reserve { Quantity = 5, Nominal = 100 }
            }
            .OrderByDescending(x => x.Nominal)
            .ToList();
            LastRecievedNominals = new List<Reserve>();
            Balance = Reserves.Sum(r=>r.Quantity*r.Nominal);
        }

        public int Balance { get; set; }
        public List<Reserve> Reserves { get; set; }
        public List<Reserve> LastRecievedNominals { get; set; }

        public bool CanPerformOperation(int withdrawAmount)
        {
            return (Balance - withdrawAmount) >= 0;
        }

        public bool PerformWithdraw(int withdrawAmount)
        {
            LastRecievedNominals.Clear();
            foreach (var item in Reserves)
            {
                int numberNominalWished = Math.DivRem(withdrawAmount, item.Nominal, out int remainder);

                if (numberNominalWished <= item.Quantity)
                {
                    withdrawAmount -= item.Nominal * numberNominalWished;
                    item.Quantity -= numberNominalWished;
                    LastRecievedNominals.Add(new Reserve { Quantity = numberNominalWished, Nominal = item.Nominal });
                    Balance -= item.Nominal * numberNominalWished;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }

    class Reserve
    {
        public int Quantity { get; set; }
        public int Nominal { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start transaction!");

            ATMBalance aTMBalance = new ATMBalance();
            bool procceed = true;

            while (procceed)
            {
                Console.WriteLine("Enter the amount to withdraw:");
                if (!int.TryParse(Console.ReadLine(), out int wishAmount))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not a valid number format!");
                    Console.ResetColor();
                }
                else
                {
                    if (aTMBalance.CanPerformOperation(wishAmount))
                    {
                        if (aTMBalance.PerformWithdraw(wishAmount))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Successfully processed.");
                            Console.ResetColor();

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            aTMBalance.LastRecievedNominals.Where(x => x.Quantity > 0).ToList().ForEach(x =>
                            {
                                Console.WriteLine($"Nominal {x.Nominal}, Quantity {x.Quantity}.");
                            });

                            Console.WriteLine($"Balance {aTMBalance.Balance}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Sorry. ATM does not have sufficient nominals");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not sufficient money on your account.");
                        Console.ResetColor();
                    }
                }

                Console.WriteLine($"Do you want to continue? Tipe \"Y\"");
                procceed = (Console.ReadLine().ToLower() == "y");
                //procceed = true;
            }

            Console.ReadKey();
        }
    }
}
