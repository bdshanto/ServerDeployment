﻿namespace ServerDeployment.Console.Helpers
{
    public class IISSiteInfo
    {
        public bool Select { get; set; }
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public string ContentSize { get; set; }

        public string State { get; set; }
    }

    public enum DeployEnum
    {
        Frontend,
        PetMatrixBackendAPI,
        ReportsViewer
    }
    public enum ProgressType
    {
        Backup,
        Report,
        Frontend,
        Backend,
        AppSettings,
        Delete

    }
}
