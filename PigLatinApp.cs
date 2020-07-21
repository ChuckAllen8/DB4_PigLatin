using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DB4_PigLatin
{
    public class PigLatinApp
    {
        private enum Case
        {
            Title,
            Lower,
            Mixed,
            Upper
        }

        private readonly List<char> vowels;
        private readonly List<char> notConverting;

        public PigLatinApp()
        {
            vowels = new List<char>()
            {
                'a',
                'e',
                'i',
                'o',
                'u',
                'A',
                'E',
                'I',
                'O',
                'U'
            };

            notConverting = new List<char>()
            {
                '@',
                '$',
                '#',
                '{',
                '}',
                '_',
                '=',
                '+',
                '(',
                ')'
            };
        }

        public void Start()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Pig Latin Translator!");
                Console.Write("Enter a line to be translated, or a file-path to display: ");
                string word = Console.ReadLine();
                Console.WriteLine();

                if(IsFileAddress(word))
                {
                    DisplayFile(word);
                }
                else
                {
                    Console.WriteLine(ConvertLine(word));
                }

                Console.WriteLine();
            } while (AnotherLine());
        }

        private bool AnotherLine()
        {
            Console.Write("Translate another line? (y/n): ");
            ConsoleKey choice = Console.ReadKey().Key;
            Console.WriteLine();
            if(choice == ConsoleKey.Y)
            {
                return true;
            }
            else if (choice == ConsoleKey.N)
            {
                return false;
            }
            else
            {
                return AnotherLine();
            }
        }

        private string ConvertLine(string paragraph)
        {
            string[] words = paragraph.Split(" ");
            StringBuilder pigLatin = new StringBuilder();
            foreach(string word in words)
            {
                pigLatin.Append(ConvertWord(word));
                pigLatin.Append(" ");
            }
            return pigLatin.ToString();
        }



        private string ConvertWord(string word)
        {
            //Check to make sure we are only trying to convert words
            //do not convert null or empty strings
            if(word == null || word.Trim() == "")
            {
                return "";
            }

            //Check cases for words to not convert
            //currently this is words that include the Non-Convert symbols
            foreach(char letter in word)
            {
                if(notConverting.Contains(letter))
                {
                    return word;
                }
            }

            string converted;
            Case wordCase = CheckCase(word);
            char[] letters = word.ToCharArray();

            if (IsSentenceEnd(word))
            {
                letters[letters.Length-1] = ' ';
                converted = Convert(letters) + word[word.Length-1];
            }
            else
            {
                converted = Convert(letters);
            }

            return MakeCase(converted, wordCase);
        }

        //should only be called with a filled array.
        private string Convert(char[] letters)
        {
            Queue<char> addToEnd = new Queue<char>();
            StringBuilder word = new StringBuilder();
            string trimmed;

            //if it starts with a vowel add 'way' to the end
            if (vowels.Contains(letters[0]))
            {
                foreach(char letter in letters)
                {
                    word.Append(letter);
                }
                trimmed = word.ToString().Trim();
                return trimmed + "way";
            }
            else
            {
                //look for the first vowel and add all letters before it to the end of the word
                for (int index = 0; index < letters.Length; index++)
                {
                    //found a vowel, exit the loop
                    if (vowels.Contains(letters[index]))
                    {
                        break;
                    }
                    else
                    {
                        //not a vowel, add it to the end, and replace with a space at the beginning.
                        addToEnd.Enqueue(letters[index]);
                        letters[index] = ' ';
                    }
                }

                //get all the letters from the first vowel to the end of the word.
                foreach (char letter in letters)
                {
                    word.Append(letter);
                }

                //take out any additional spaces as a result of the logic
                trimmed = word.ToString().Trim();
                word.Clear();
                word.Append(trimmed);

                //add in all of the letters that were moved to the end.
                while (addToEnd.TryDequeue(out char letter))
                {
                    word.Append(letter);
                }

                //take out any additional spaces as a result of the logic
                trimmed = word.ToString().Trim();
                return trimmed + "ay";
            }
        }

        private Case CheckCase(string word)
        {
            char[] letters = word.ToCharArray();
            
            //booleans to keep track of the word.
            bool allUpper = true;
            bool allLower = true;
            bool titleCase = true;

            for(int index = 0; index < letters.Length; index++)
            {
                if(char.IsLower(letters[index]))
                {
                    allUpper = false;
                }

                if (char.IsUpper(letters[index]))
                {
                    allLower = false;
                }

                if (index == 0 && char.IsLower(letters[index]))
                {
                    titleCase = false;
                }
                if(index > 0 && char.IsUpper(letters[index]))
                {
                    titleCase = false;
                }
            }

            //It's a mixed case word, with no clear indication, return mixed
            if(!titleCase && !allUpper && !allLower)
            {
                return Case.Mixed;
            }

            //If it is a title case word return that
            if (titleCase)
            {
                return Case.Title;
            }

            if (allUpper)
            {
                return Case.Upper;
            }
            //if it is not upper, title, or mixed case it must be lower
            return Case.Lower;
        }

        private string MakeCase(string word, Case toCase)
        {
            switch (toCase)
            {
                //mixed case is being treated as lower case
                case Case.Mixed:
                case Case.Lower:
                    return word.ToLower();
                case Case.Upper:
                    return word.ToUpper();
                default:
                    return TitleCase(word);
            }
        }

        private string TitleCase(string word)
        {
            char[] letters = word.ToCharArray();
            StringBuilder titleCase = new StringBuilder();
            titleCase.Append(word[0].ToString().ToUpper());
            for (int index = 1; index < letters.Length; index++)
            {
                titleCase.Append(word[index].ToString().ToLower());
            }
            return titleCase.ToString();
        }

        private bool IsSentenceEnd(string word)
        {
            if(char.IsPunctuation(word[word.Length - 1]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsFileAddress(string fileName)
        {
            return File.Exists(fileName);
        }

        private void DisplayFile(string fileName)
        {
            StreamReader file = new StreamReader(fileName);

            while(!file.EndOfStream)
            {
                Console.WriteLine(ConvertLine(file.ReadLine()));
            }

            file.Close();
        }
    }
}
