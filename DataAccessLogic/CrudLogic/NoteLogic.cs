using DataAccessLogic.DatabaseModels;
using DataAccessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLogic.CrudLogic
{
    public class NoteLogic : ICrudLogic<Note>
    {
        private readonly ApplicationContext context;

        public NoteLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task Create(Note model)
        {
            if (string.IsNullOrWhiteSpace(model.AntiquesId))
            {
                throw new Exception("Антиквариат не определен");
            }

            if (string.IsNullOrWhiteSpace(model.Text))
            {
                throw new Exception("Отсутствует описание");
            }

            model.Id = Guid.NewGuid().ToString();
            model.Date = DateTime.Now;

            await context.Notes.AddAsync(model);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Note model)
        {
            if (string.IsNullOrWhiteSpace(model.AntiquesId))
            {
                throw new Exception("Антиквариат не определен");
            }

            Note noteToDelete = await context.Notes.FirstOrDefaultAsync(note =>
            note.AntiquesId == model.AntiquesId);

            if (noteToDelete != null)
            {
                context.Notes.Remove(noteToDelete);
                await context.SaveChangesAsync();
            }
        }

        public Task<List<Note>> Read(Note model)
        {
            throw new NotImplementedException();
        }

        public Task Update(Note model)
        {
            throw new NotImplementedException();
        }
    }
}