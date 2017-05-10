using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MaterialDesignColors.WpfExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {

                //First get the 'user-scoped' storage information location reference in the assembly
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
                //create a stream writer object to write content in the location
                StreamWriter srWriter = new StreamWriter(new IsolatedStorageFileStream("isotest", FileMode.Create, isolatedStorage));
                //check the Application property collection contains any values.
                if (App.Current.Properties[0] != null)
                {
                    //wriet to the isolated storage created in the above code section.
                    srWriter.WriteLine(App.Current.Properties[0].ToString());
                    srWriter.WriteLine(App.Current.Properties[1].ToString());

                }

                srWriter.Flush();
                srWriter.Close();
            }
            catch (System.Security.SecurityException sx)
            {
                MessageBox.Show(sx.Message);
                throw;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //First get the 'user-scoped' storage information location reference in the assembly
                IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
                //create a stream reader object to read content from the created isolated location
                StreamReader srReader = new StreamReader(new IsolatedStorageFileStream("isotest", FileMode.OpenOrCreate, isolatedStorage));

                //Open the isolated storage
                if (srReader == null)
                {
                    MessageBox.Show("No Data stored!");
                }
                else
                {
                    //MessageBox.Show(stateReader.ReadLine());
                    int i = 0;
                    while (!srReader.EndOfStream)
                    {
                        string item = srReader.ReadLine();
                        App.Current.Properties[i] = item;
                        i++;
                    }
                }
                //close reader
                srReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
    }
}

