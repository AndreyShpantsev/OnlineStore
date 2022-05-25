using DataAccessLogic.DatabaseModels;
using DataAccessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLogic.CrudLogic
{
    public class SavedListLogic : ISavedLogic
    {
        private readonly ApplicationContext context;

        public SavedListLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task Create(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
            {
                throw new Exception("Пользователь не определен");
            }

            if (!context.SavedLists.Any(savedList =>
            savedList.UserId == user.Id))
            {
                SavedList newList = new SavedList
                {
                    Id = Guid.NewGuid().ToString(),
                    User = user
                };
                await context.SavedLists.AddAsync(newList);
                await context.SaveChangesAsync();
            }
        }

        public async Task Remove(User user, Antiques savedAntiques)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
            {
                throw new Exception("Пользователь не определен");
            }

            if (savedAntiques == null || string.IsNullOrWhiteSpace(savedAntiques.Id))
            {
                throw new Exception("Антиквариат не определен");
            }

            SavedList existingList = await context.SavedLists
                .Include(list => list.Antiques)
                .FirstOrDefaultAsync(list => list.UserId == user.Id);

            if (existingList == null)
            {
                throw new Exception("Список избранного не найден");
            }

            Antiques antiquesToRemove = existingList.Antiques
                .Find(antiques => antiques.Id == savedAntiques.Id);

            existingList.Antiques.Remove(antiquesToRemove);

            await context.SaveChangesAsync();
        }

        public async Task<SavedList> Read(User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
            {
                throw new Exception("Пользователь не определен");
            }

            return await context.SavedLists.Include(savedList => savedList.Antiques)
            .FirstOrDefaultAsync(savedList => savedList.UserId == user.Id);
        }

        public async Task Add(User user, Antiques savedAntiques)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Id))
            {
                throw new Exception("Пользователь не определен");
            }

            if (savedAntiques == null || string.IsNullOrWhiteSpace(savedAntiques.Id))
            {
                throw new Exception("Антиквариат не определен");
            }

            SavedList existingList = await context.SavedLists
                .Include(list => list.Antiques)
                .FirstOrDefaultAsync(list => list.UserId == user.Id);

            Antiques antiquesToSave = await context.Antiques.FindAsync(savedAntiques.Id);

            if (existingList == null)
            {
                throw new Exception("Список избранного не найден");
            }

            existingList.Antiques.Add(antiquesToSave);

            await context.SaveChangesAsync();
        }
    }
}