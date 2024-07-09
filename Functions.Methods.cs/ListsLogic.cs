using System.Collections.Generic;

namespace TaskManagement
{
    public class ListManager
    {
        private Dictionary<string, List<string>> lists;

        public ListManager()
        {
            lists = new Dictionary<string, List<string>>();
        }

        public bool ListExists(string listName)
        {
            return lists.ContainsKey(listName);
        }

        public void CreateList(string listName)
        {
            if (!ListExists(listName))
            {
                lists[listName] = new List<string>();
            }
        }

        public void AddItemToList(string listName, string item)
        {
            if (ListExists(listName))
            {
                lists[listName].Add(item);
            }
        }

        public bool RemoveItemFromList(string listName, string item)
        {
            if (ListExists(listName))
            {
                return lists[listName].Remove(item);
            }
            return false;
        }

        public List<string> GetItemsInList(string listName)
        {
            if (ListExists(listName))
            {
                return lists[listName];
            }
            return new List<string>();
        }

        public List<string> GetAllLists()
        {
            return new List<string>(lists.Keys);
        }

        public Dictionary<string, List<string>> GetAllListsWithItems()
        {
            return lists;
        }

        public void LoadLists(Dictionary<string, List<string>> loadedLists)
        {
            lists = loadedLists;
        }
    }
}

