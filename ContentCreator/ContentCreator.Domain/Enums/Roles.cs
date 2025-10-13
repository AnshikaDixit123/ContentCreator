namespace ContentCreator.Domain.Enums
{
    public enum Roles
    {
        //these are the default roles, if you are doing any changes in the future do it mindfully as these roles are registered in the DB.
        SuperAdmin,
        User,
        Writer,
        Photographer,
        Videographer,
        Singer
    }
}
