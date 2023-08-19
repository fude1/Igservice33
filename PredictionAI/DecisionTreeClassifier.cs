using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace IGServiceDevelop.PredictionAI
{
  
    internal class DecisionTreeClassifier
    {
        public const string LIBS_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio\Shared\Python39_64";
        public const string ASSEMBLY_FILE = "python39.dll";
        string targetPath = Path.Combine("", LIBS_PATH, ASSEMBLY_FILE);
   
        // Metodo per addestrare il modello di regressione lineare
        public void TrainModel()
        {
            Assembly oAssembly = Assembly.GetExecutingAssembly();
            oAssembly = Assembly.LoadFrom(targetPath);
  
            PythonEngine.Initialize();

            // Import required Python modules
            dynamic DecisionTreeClassifier = Py.Import("sklearn.tree");
            dynamic pd = Py.Import("pandas");
            dynamic pk = Py.Import("pickle");
            dynamic model_selection = Py.Import("train_test_split");
            dynamic metrics = Py.Import("sklearn.metrics");
            dynamic open = Py.Import("builtins");

            dynamic stock_data = pd.read_csv(@"c:/temp/FUDEmodellotraining.csv");

            dynamic X = stock_data["close", "RSI", "BB", "RSI BUY", "B BUY"];
            dynamic y = stock_data["BUY"];

            // Split the dataset into training and testing sets
            dynamic X_train, X_test, y_train, y_test;
            dynamic train_test_split = model_selection.train_test_split;
            dynamic train_test_split_result = train_test_split(X, y, test_size: 0.2);
         //  (X_train, X_test, y_train, y_test) = train_test_split_result;

            // Create and train the DecisionTreeClassifier
            dynamic clf = DecisionTreeClassifier.DecisionTreeClassifier();
           // clf.fit(X_train, y_train);

            // Make predictions on the test set
         //   dynamic y_pred = clf.predict(X_test);

            // Calculate accuracy
           // dynamic accuracy = metrics.accuracy_score(y_test, y_pred);
          // Console.WriteLine("Accuracy: " + accuracy);

            string modelPath = @"C:\inetpub\IGWebApi\Data\model.pkl";
            using (Py.GIL()) // Acquire the Python Global Interpreter Lock (GIL)
            {
                dynamic file = open(modelPath, "wb");
                pk.dump(clf, file);
                file.close();
            }

            // Cleanup Python runtime
            PythonEngine.Shutdown();

        }




        // Metodo per prevedere il valore delle azioni
        public void Predict()
        {

        }


    }
}
