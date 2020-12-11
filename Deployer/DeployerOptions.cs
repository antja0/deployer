namespace Deployer
{
    public class DeployerOptions
    {
        /// <summary>
        /// Folder (at the deployer server) where applications are built.
        /// </summary>
        public string TempBuildsFolderPath { get; set; }

        /// <summary>
        /// Folder (at the deployer server) where built versions are stored.
        /// </summary>
        public string VersionFolderPath { get; set; }

        /// <summary>
        /// Folder (at the deployer server) where are build scripts are located.
        /// </summary>
        public string ScriptsFolderPath { get; set; }
    }
}
