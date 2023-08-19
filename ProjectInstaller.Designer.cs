namespace IGService3
{
   partial class ProjectInstaller
   {
      /// <summary>
      /// Variabile di progettazione necessaria.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Liberare le risorse in uso.
      /// </summary>
      /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Codice generato da Progettazione componenti

      /// <summary>
      /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
      /// il contenuto del metodo con l'editor di codice.
      /// </summary>
      private void InitializeComponent()
      {
         this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
         this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
         // 
         // serviceInstaller1
         // 
         this.serviceInstaller1.DisplayName = "IG Service3";
         this.serviceInstaller1.ServiceName = "IGService3";
         this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
         // 
         // serviceProcessInstaller1
         // 
         this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
         this.serviceProcessInstaller1.Password = null;
         this.serviceProcessInstaller1.Username = null;
         // 
         // ProjectInstaller
         // 
         this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceInstaller1,
            this.serviceProcessInstaller1});

      }

      #endregion

      private System.ServiceProcess.ServiceInstaller serviceInstaller1;
      private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
   }
}