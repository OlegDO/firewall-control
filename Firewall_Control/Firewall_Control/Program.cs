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
                   string profile = ConsoleWriter.Default.PrintQuestion("Enter type of profile");
                   if(profile.ToUpper() == "PUBLIC" || profile.ToUpper() == "DOMAIN" || profile.ToUpper() == "PRIVATE")
                   {
                       FirewallProfiles firewallProfile = FirewallProfiles.Public;
                       if(profile.ToUpper() == "PUBLIC")
                       {
                           firewallProfile = FirewallProfiles.Public;
                       }

                       if(profile.ToUpper() == "DOMAIN")
                       {
                           firewallProfile = FirewallProfiles.Domain;
                       }

                       if(profile.ToUpper() == "PRIVATE")
                       {
                           firewallProfile = FirewallProfiles.Private;
                       }
                       
                       // Имя правила
                       string name = ConsoleWriter.Default.PrintQuestion("Enter name of rule");
                       if(name != "")
                       {
                            // Тип доступа
                       FirewallAction firewallaction = FirewallAction.Block;
                       string action = ConsoleWriter.Default.PrintQuestion("Enter type of acces");
                       if(action.ToUpper() == "ALLOW" || action.ToUpper() == "BLOCK")
                       {
                           if(action.ToUpper() == "ALLOW")
                       {
                           firewallaction = FirewallAction.Allow;
                       }
                               if (action.ToUpper() == "BLOCK")
                       {
                           firewallaction = FirewallAction.Block;
                       }

                           string fullPath = ConsoleWriter.Default.PrintQuestion("Enter full path of exe file");

                           var rule = firewallInstance.CreateApplicationRule(
                            firewallProfile,
                            @$"{name}",
                            firewallaction,
                           @$"{fullPath}"
                        );
                        rule.Direction = FirewallDirection.Outbound;
                        firewallInstance.Rules.Add(rule);
                        ConsoleWriter.Default.PrintSuccess("Rule successfully aded");
                       } else
                           {
                           ConsoleWriter.Default.PrintError("This type of acces is invalid");

                       }
                       } else
                       {
                           ConsoleWriter.Default.PrintError("This name of rule is invalid");
                       }

                   } else
                       {
                           ConsoleWriter.Default.PrintError("This profile name is invalid");
                       }

                    ConsoleWriter.Default.PrintMessage("Press any key to get one step back.");
                    
                    // Вsозврат
                    Console.ReadKey();
                }),
              new ConsoleNavigationItem("Create port rule", (i, item) => {
                  string name = ConsoleWriter.Default.PrintQuestion("Enter name of port rule");
                  if(name != "")
                  {
                     string action = ConsoleWriter.Default.PrintQuestion("Enter type of access");
                     FirewallAction firewallAction = FirewallAction.Block;

                      if(action.ToUpper() == "ALLOW" || action.ToUpper() == "BLOCK")
                      {
                          if(action.ToUpper() == "ALLOW")
                          {
                              firewallAction = FirewallAction.Allow;
                          }

                          if(action.ToUpper() == "BLOCK")
                          {
                              firewallAction = FirewallAction.Block;
                          }

                            string port = ConsoleWriter.Default.PrintQuestion("Enter port");
                            ushort finallyPort = Convert.ToUInt16(port);
                            string protocol = ConsoleWriter.Default.PrintQuestion("Enter protocol");
                          FirewallProtocol firewallProtocol = FirewallProtocol.TCP;

                             if(protocol.ToUpper() == "UDP" || protocol.ToUpper() == "TCP")
                          {
                              if(protocol.ToUpper() == "UDP")
                              {
                                  firewallProtocol = FirewallProtocol.UDP;
                              }

                              if(protocol.ToUpper() == "TCP")
                              {
                                  firewallProtocol = FirewallProtocol.TCP;
                              }
                              var rule = firewallInstance.CreatePortRule(
                            @$"{name}",
                            firewallAction,
                            finallyPort,
                            firewallProtocol
                        );
                        firewallInstance.Rules.Add(rule);
                              ConsoleWriter.Default.PrintSuccess("Protocol rule successfully aded");
                          } else {
                           ConsoleWriter.Default.PrintError("This protocol is invalid");
                      }

                      } else
                      {
                           ConsoleWriter.Default.PrintError("This type of acces is invalid");
                      }

                  } else
                  {
                      ConsoleWriter.Default.PrintError("This name of port rule is invalid");
                  }
                    
                    // Возврат
                    Console.ReadKey();
                }),
              new ConsoleNavigationItem("Delete rule", (i, item) => {
                  string deleted = ConsoleWriter.Default.PrintQuestion("Enter name of rule");
                   var rule = firewallInstance.Rules.SingleOrDefault(r => r.Name == deleted);
                       if (rule != null)
                        {
                            firewallInstance.Rules.Remove(rule);
                                ConsoleWriter.Default.PrintSuccess("Rule was successfully deleted");
                        } else
                  {
                      ConsoleWriter.Default.PrintError("This name rule is invalid");
                  }

              })
            }, "Select an execution path.");


        }
    }
}

