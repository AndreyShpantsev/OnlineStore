using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLogic.CrudLogic
{
    public class AntiquesLogic : ICrudLogic<Antiques>, IPagination<Antiques>
    {
        private readonly ApplicationContext context;

        public AntiquesLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task Create(Antiques model)
        {
            if (model.User == null || string.IsNullOrWhiteSpace(model.User.UserName))
            {
                throw new Exception("Пользователь не определен");
            }

            Antiques sameAntiques = await context.Antiques
                .Include(antiques => antiques.User)
                .FirstOrDefaultAsync(antiques =>
                antiques.User.UserName == model.User.UserName && antiques.Name == model.Name);
            if (sameAntiques != null)
            {
                throw new Exception("Уже есть антиквариат с таким названием");
            }

            model.Status = AntiquesStatusProvider.GetOnModerationStatus();
            model.Id = Guid.NewGuid().ToString();
            model.Date = DateTime.Now;
            model.User = await context.Users.FirstAsync(user =>
            user.UserName == model.User.UserName);

            await context.Antiques.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Antiques model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new Exception("Антиквариат не определен");
            }

            Antiques toDelete = await context.Antiques.FirstOrDefaultAsync(antiques =>
            antiques.Id == model.Id);
            if (toDelete == null)
            {
                throw new Exception("Антиквариат не найден");
            }

            context.Antiques.Remove(toDelete);
            await context.SaveChangesAsync();
        }

        public async Task Update(Antiques model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                throw new Exception("Антиквариат не определен");
            }

            if (model.User != null)
            {
                Antiques sameAntiques = await context.Antiques
                .Include(antiques => antiques.User)
                .FirstOrDefaultAsync(antiques =>
                antiques.User.UserName == model.User.UserName && antiques.Name == model.Name);
                if (sameAntiques != null)
                {
                    throw new Exception("Уже есть антиквариат с таким названием");
                }
            }

            Antiques toUpdate = await context.Antiques.FirstOrDefaultAsync(antiques =>
            antiques.Id == model.Id);
            if (toUpdate == null)
            {
                throw new Exception("Антиквариат не найден");
            }
            toUpdate.Status = string.IsNullOrWhiteSpace(model.Status) ? toUpdate.Status : model.Status;
            toUpdate.Name = string.IsNullOrWhiteSpace(model.Name) ? toUpdate.Name : model.Name;
            toUpdate.Description = string.IsNullOrWhiteSpace(model.Description) ? toUpdate.Description : model.Description;
            toUpdate.PhotoSrc = string.IsNullOrWhiteSpace(model.PhotoSrc) ? toUpdate.PhotoSrc : model.PhotoSrc;

            await context.SaveChangesAsync();
        }

        public async Task<List<Antiques>> Read(Antiques model)
        {
            return await context.Antiques.Include(antiques => antiques.User).Include(antiques => antiques.Note).Where(antiques => model == null
            || model.User != null && !string.IsNullOrWhiteSpace(model.User.UserName) && antiques.User.UserName == model.User.UserName
            || !string.IsNullOrWhiteSpace(model.Id) && antiques.Id == model.Id
            || !string.IsNullOrWhiteSpace(model.Status) && antiques.Status == model.Status)
            .ToListAsync();
        }

        public async Task<List<Antiques>> GetPage(int pageNumber, Antiques model)
        {
            return await context.Antiques.Include(antiques => antiques.User).Where(antiques => model == null
            || !string.IsNullOrWhiteSpace(model.Status) && antiques.Status == model.Status
            || model.User != null && antiques.User == model.User)
            .Skip((pageNumber <= 0 ? 0 : pageNumber - 1) *
            ApplicationConstantsProvider.GetPageSize())
            .Take(ApplicationConstantsProvider.GetPageSize())
            .ToListAsync();
        }

        public async Task<int> GetCount(Antiques model)
        {
            return await context.Antiques.CountAsync(antiques => model == null
            || !string.IsNullOrWhiteSpace(model.Status) && antiques.Status == model.Status
            || model.User != null && antiques.User == model.User);
        }
    }
}