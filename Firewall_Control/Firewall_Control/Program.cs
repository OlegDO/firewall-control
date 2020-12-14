using ConsoleUtilities;
using System;
using System.Linq;
using WindowsFirewallHelper;

namespace Firewall_Control
{
    internal class Program
    {
        private static void Main()
        {
            ConsoleWriter.Default.PrintMessage("Firewall Control");

            // Информация о версии  брандмауэра
            ConsoleWriter.Default.PrintMessage($"Firewall Version: {FirewallManager.Version}");

            // Экземпляр ('пульт') управления брандмауэром
            var firewallInstance = FirewallManager.Instance;
            ConsoleWriter.Default.PrintMessage($"Type of control: {firewallInstance.Name}");

            // Если версия брандмауэра неизвестная или недействительна
            if (FirewallManager.Version == FirewallAPIVersion.None)
            {
                ConsoleWriter.Default.PrintMessage("Press any key to exit.");
                Console.ReadKey();

                // То выход
                return;
            }

            // Панель навигации
            // Генеральная консоль
            ConsoleNavigation.Default.PrintNavigation(new[]
            {   
                // Консоль для профилей
                new ConsoleNavigationItem("Profiles", (i, item) =>
                {   
                    // Консоль подпунктов профилей
                    ConsoleNavigation.Default.PrintNavigation(

                        // Объект кладем в массив
                        firewallInstance.Profiles.ToArray(), (i1, profile) =>

                        {   // Выводим данные о конкретном профиле
                            ConsoleWriter.Default.WriteObject(profile);
                            ConsoleWriter.Default.PrintMessage("Press any key to get one step back.");

                            // Возврат
                            Console.ReadKey();
                        },
                        "Select a profile to view its settings."
                    );
                }),

                // Консоль для правил
                new ConsoleNavigationItem("Rules", (i, item) =>
                {   
                    // Консоль всех правил
                    ConsoleNavigation.Default.PrintNavigation(

                        // Сортируем правила в алфавитном порядке
                        // Каждое правило (объект) кладем в массив
                        firewallInstance.Rules.OrderBy((rule) => rule.FriendlyName).ToArray(), (i1, rule) =>
                        {   
                            // Вывод правила
                            ConsoleWriter.Default.WriteObject(rule);
                            ConsoleWriter.Default.PrintMessage("Press any key to get one step back.");

                            // Возврат
                            Console.ReadKey();
                        },
                        "Select a rule to view its settings."
                    );
                }),

               new ConsoleNavigationItem("Create rule", (i, item) => {
                  
                   // Задание правилу профиля
                   string profile = ConsoleWriter.Default.PrintQuestion("Enter type of profile: ");
                   if(profile == "Public" || profile == "Domain" || profile == "Private")
                   {
                       FirewallProfiles firewallProfile = FirewallProfiles.Public;
                       if(profile == "Public")
                       {
                           firewallProfile = FirewallProfiles.Public;
                       } else if(profile == "Domain")
                       {
                           firewallProfile = FirewallProfiles.Domain;
                       } else if(profile == "Private")
                       {
                           firewallProfile = FirewallProfiles.Private;
                       } 

                       // Имя правила
                       string name = ConsoleWriter.Default.PrintQuestion("Enter name of rule: ");
                       
                       // Тип доступа
                       FirewallAction firewallaction = FirewallAction.Block;
                       string action = ConsoleWriter.Default.PrintQuestion("Enter type of acces: ");
                       if(action == "Allow" || action == "Block")
                       {
                           if(action == "Allow")
                       {
                           firewallaction = FirewallAction.Allow;
                       } else if (action == "Block")
                       {
                           firewallaction = FirewallAction.Block;
                       } else
                       {
                           ConsoleWriter.Default.PrintError("This type of acces is invalid");

                       }
                           string fullPath = ConsoleWriter.Default.PrintQuestion("Enter full path of exe file: ");

                           var rule = firewallInstance.CreateApplicationRule(
                            firewallProfile,
                            @$"{name}",
                            firewallaction,
                           @$"{fullPath}"
                        );
                        rule.Direction = FirewallDirection.Outbound;
                        firewallInstance.Rules.Add(rule);
                        ConsoleWriter.Default.PrintSuccess("Rule successfully aded");
                       }
                   } else
                       {
                           ConsoleWriter.Default.PrintError("This profile name is invalid");
                       }

                    ConsoleWriter.Default.PrintMessage("Press any key to get one step back.");
                    
                    // Возврат
                    Console.ReadKey();
                }),
              new ConsoleNavigationItem("Create port rule", (i, item) => {

                    ConsoleWriter.Default.PrintMessage("Press any key to get one step back.");
                     var rule = firewallInstance.CreatePortRule(
                            @"Port 80",
                            FirewallAction.Allow,
                            80,
                            FirewallProtocol.TCP
                        );
            firewallInstance.Rules.Add(rule);
                    // Возврат
                    Console.ReadKey();
                })
            }, "Select an execution path.");


        }
    }
}
