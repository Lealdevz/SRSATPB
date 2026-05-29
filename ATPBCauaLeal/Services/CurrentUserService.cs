using ATPBCauaLeal.Models;

namespace ATPBCauaLeal.Services;

public class CurrentUserService
{
    public UserRole Role { get; set; } = UserRole.Aluno;

    public Aluno? Aluno { get; set; }
    public Professor? Professor { get; set; }
    public Admin? Administrador { get; set; }

    public void SetAluno(Aluno aluno)
    {
        Role = UserRole.Aluno;
        Aluno = aluno;
    }

    public void SetProfessor(Professor professor)
    {
        Role = UserRole.Professor;
        Professor = professor;
    }

    public void SetAdministrador(Admin admin)
    {
        Role = UserRole.Admin;
        Administrador = admin;
    }
}
