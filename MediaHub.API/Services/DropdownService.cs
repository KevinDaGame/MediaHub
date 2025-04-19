using System;
using System.Collections.Generic;
using MediaHub.API.Components.Dropdown;

namespace MediaHub.API.Services
{
    public class DropdownService
    {
        private readonly List<Dropdown> _openDropdowns = new();
        
        public event Action? OnDropdownsChanged;
        
        public void RegisterDropdown(Dropdown dropdown)
        {
            if (!_openDropdowns.Contains(dropdown))
            {
                _openDropdowns.Add(dropdown);
                OnDropdownsChanged?.Invoke();
            }
        }
        
        public void UnregisterDropdown(Dropdown dropdown)
        {
            if (_openDropdowns.Contains(dropdown))
            {
                _openDropdowns.Remove(dropdown);
                OnDropdownsChanged?.Invoke();
            }
        }
        
        public void CloseAllDropdowns(Dropdown? except = null)
        {
            foreach (var dropdown in _openDropdowns.ToArray())
            {
                if (dropdown != except)
                {
                    dropdown.Close();
                }
            }
        }
        
        public void CloseAllDropdowns()
        {
            foreach (var dropdown in _openDropdowns.ToArray())
            {
                dropdown.Close();
            }
        }
    }
}