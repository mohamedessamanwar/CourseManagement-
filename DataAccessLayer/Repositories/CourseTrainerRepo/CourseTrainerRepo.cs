﻿using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.CourseTrainerRepo
{
    public class CourseTrainerRepo : ICourseTrainerRepo
    {
        private readonly ApplicationDbContext applicationDbContext;

        public CourseTrainerRepo(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        public async Task<CourseTrainer?> GetCourseTrainer(int TrainerId, int CourseId)
        {
            return await applicationDbContext.CourseTrainers
                 .Where(predicate: c => c.TrainerId == TrainerId && c.CourseId == CourseId)
                 .FirstOrDefaultAsync();
        }
        public async Task Add(CourseTrainer courseTrainer)
        {
            await applicationDbContext.CourseTrainers.AddAsync(courseTrainer);
        }

        public async Task<int> CompleteAsync()
        {
            return await applicationDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseTrainer>> GetCourseTrainers(int trainerId)
        {
            return await applicationDbContext.CourseTrainers.AsNoTracking().
                Include(e => e.Course).
                Where(e => e.TrainerId == trainerId).ToListAsync();

        }

        public async Task<IEnumerable<CourseTrainer>> GetCourseTrainersWithPayments()
        {
            return await applicationDbContext.CourseTrainers.AsNoTracking().
                Include(e => e.Course).Include(e => e.Trainer)
                .Include(e => e.Payments).OrderBy(e => e.Trainer.FullName)
                 .ToListAsync();

        }


    }
}
