﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using StudentDetails.Models;

namespace StudentDetails.Controllers
{
    public class StudentsController : ApiController
    {
        private StudentDetailsContext db = new StudentDetailsContext();

        // GET: api/Students
        public IQueryable<StudentDTO> GetStudents()
        {
            var students = from s in db.Students
                           select new StudentDTO()
                           {
                               Id = s.Id,
                               FirstName = s.FirstName,
                               DepartmentName = s.Department.Name
                           };
            return students;
        }

        // GET: api/Students/5
        [ResponseType(typeof(StudentDetailsDTO))]
        public async Task<IHttpActionResult> GetStudent(int id)
        {
            var student = await db.Students.Include(s => s.Department).Select(s =>
                new StudentDetailsDTO()
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    YearEnrolled = s.YearEnrolled,
                    DateOfBirth = s.DateOfBirth,
                    DepartmentId = s.Department.Id,
                    DepartmentName = s.Department.Name,
                    Email = s.Email
                }).SingleOrDefaultAsync(s=>s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/Students/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutStudent(int id, Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != student.Id)
            {
                return BadRequest();
            }

            db.Entry(student).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Students
        [ResponseType(typeof(Student))]
        public async Task<IHttpActionResult> PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Students.Add(student);
            await db.SaveChangesAsync();

            db.Entry(student).Reference(x => x.Department).Load();

            var dto = new StudentDTO()
            {
                Id = student.Id,
                FirstName = student.FirstName,
                DepartmentName = student.Department.Name
            };

            return CreatedAtRoute("DefaultApi", new { id = student.Id }, dto);
        }

        // DELETE: api/Students/5
        [ResponseType(typeof(Student))]
        public async Task<IHttpActionResult> DeleteStudent(int id)
        {
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            db.Students.Remove(student);
            await db.SaveChangesAsync();

            return Ok(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentExists(int id)
        {
            return db.Students.Count(e => e.Id == id) > 0;
        }
    }
}