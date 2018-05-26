using System;

namespace CreateMask.Containers
{
    [Serializable]
    public class GitHubRepoInfo
    {
        public string Name { get; private set; }
        public string Owner { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public GitHubRepoInfo(string name, string owner, string username, string password)
        {
            Name = name;
            Owner = owner;
            Username = username;
            Password = password;
        }
    }
}
