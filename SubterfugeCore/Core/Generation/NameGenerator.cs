﻿using System;
using System.Collections.Generic;
using System.Linq;
using SubterfugeCore.Core.Timing;

namespace SubterfugeCore.Core.Generation
{
    public class NameGenerator
    {
        private SeededRandom seeder;

        private List<string> _outpostNames;
        private List<string> _fallbackNames = new List<string>();
        private List<string> _selectedNames = new List<string>();
        
        /// <summary>
        /// Creates an instance of the name generator using an already generated seeded random tool.
        /// </summary>
        /// <param name="seeder">The seeder to use for random name selection</param>
        public NameGenerator(SeededRandom seeder)
        {
            this.seeder = seeder;
            this.populateNames();
        }

        /// <summary>
        /// Sets the list of outpost names to be pulled from.
        /// </summary>
        public void populateNames()
        {
            _outpostNames = new List<string>() {
            "Rokovo",
            "Latvia",
            "Pichu",  // Pokemon easer egg
            "Shiloh", // My dog. RIP <3
            "Dozer",
            "Nautilus",
            "Minceraft", // Pretty obvious
            "Sans", // Undertale
            "London",
            "Helsinki",
            "Subterfuge",
            "Lyons",
            "Norris",
            "Hooper",
            "Glover",
            "Mills",
            };
            
            // As an additional measure,
            // more names are derived from the existing names.
            this._generateDerivedOutpostNames();
        }

        /// <summary>
        /// Generates a random outpost name for an outpost
        /// </summary>
        /// <returns>The random outpost name</returns>
        public string GetRandomName()
        {
            // If there are pre-generated outposts to be selected from,
            // Choose one of them.
            if (_outpostNames.Count > 0)
            {
                int selection = seeder.NextRand(0, _outpostNames.Count - 1);
                string name = _outpostNames[selection];
                _outpostNames.Remove(name);
                _selectedNames.Add(name);
                return name;
            }
            
            // If there are some derived names to pick from, select them
            if (_fallbackNames.Count > 0)
            {
                int selection = seeder.NextRand(0, _outpostNames.Count - 1);
                string name = _fallbackNames[selection];
                _fallbackNames.Remove(name);
                _selectedNames.Add(name);
                return name;
            }
            
            // Otherwise, generate a random shuffled string
            // Try 10 scrambles before giving up.
            int scrambleCounter = 0;
            while (scrambleCounter < 10)
            {
                // Pick a random name from the list of names.
                int randomName = seeder.NextRand(0, _selectedNames.Count - 1);
                // Shuffle the letters in the outpost name.
                string shuffled = this._shuffleString(_selectedNames[randomName]);
                if (!_selectedNames.Contains(shuffled))
                {
                    _selectedNames.Add(shuffled);
                    return shuffled;
                }

                scrambleCounter++;
            }

            // As a final measure, if a string couldn't be derived, generate a random name.
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string generated = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[seeder.NextRand(0, s.Length)]).ToArray());
            
            return this._capitolizeFirstLetter(generated);
        }

        /// <summary>
        /// Determines if the name generator has default names able to be generated.
        /// </summary>
        /// <returns>If the default set of names has been exhausted</returns>
        public bool HasNames()
        {
            return _outpostNames.Count > 0;
        }

        /// <summary>
        /// Randomly shuffles the letters within a string
        /// </summary>
        /// <param name="str">The string to shuffle</param>
        /// <returns>The shuffled string.</returns>
        private string _shuffleString(string str)
        {
            var list = new SortedList<int,char>();
            foreach (var c in str)
            {
                int position = seeder.NextRand(0, 10000);
                while (list.ContainsKey(position))
                {
                    position = seeder.NextRand(0, 10000);
                }
                list.Add(position, c);
            }
            
            string generated = new string(list.Values.ToArray());
            return this._capitolizeFirstLetter(generated);
        }

        /// <summary>
        /// Generates a list of fallback names by deriving names from the default list.
        /// </summary>
        private void _generateDerivedOutpostNames()
        {
            List<string> derived = new List<string>();
            foreach(string deriveFrom in _outpostNames)
            {
                foreach (string appendFrom in _outpostNames)
                {
                    if (appendFrom != deriveFrom)
                    {
                        // Swap first letter
                        string derivedString = appendFrom.Substring(0, 1) + deriveFrom.Substring(1);
                        derivedString = this._capitolizeFirstLetter(derivedString);
                        if (!_outpostNames.Contains(derivedString) && !derived.Contains(derivedString))
                        {
                            derived.Add(derivedString);
                        }
                        
                        // Swap first 3 letters
                        derivedString = appendFrom.Substring(0, 3) + deriveFrom.Substring(3);
                        derivedString = this._capitolizeFirstLetter(derivedString);
                        if (!_outpostNames.Contains(derivedString) && !derived.Contains(derivedString))
                        {
                            derived.Add(derivedString);
                        }
                    }
                }
            }
            // Add to a fallback list instead of the default list.
            // This ensures that the normal outpost list names will get selected first.
            _fallbackNames.AddRange(derived);
        }

        /// <summary>
        /// Returns a string of the form "Abcdefghi", where the first is capitol and all others are lower case
        /// </summary>
        /// <param name="str">The input string to format</param>
        /// <returns>Formatted string</returns>
        private string _capitolizeFirstLetter(string str)
        {
            str = str.ToLower();
            if (str.Length == 0)
                return str;
            if (str.Length == 1)
                return Char.ToUpper(str[0]).ToString();
            return char.ToUpper(str[0]) + str.Substring(1);
        }
        
    }
}