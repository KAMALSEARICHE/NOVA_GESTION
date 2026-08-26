using System;
using System.Collections.Generic;

namespace NovaGestion.Data
{
    /// <summary>
    /// تمثيل مبسّط لعقد داخل الذاكرة (سيُستبدل لاحقاً بجدول قاعدة بيانات حقيقي).
    /// </summary>
    public class ContratItem
    {
        public string NumContrat = "";
        public string RaisonSociale = "";
        public string Type = "";
        public string Statut = "";
        public decimal MontantTTC;
        public decimal FactureDA;
        public DateTime DateContrat;
    }

    /// <summary>
    /// تمثيل مبسّط لملحق (Avenant) مرتبط بعقد موجود.
    /// </summary>
    public class AvenantItem
    {
        public string NumAvenant = "";
        public string NumContratLie = "";
        public string RaisonSociale = "";
        public string TypeModification = "";
        public DateTime DateAvenant;
        public string AncienMontant = "";
        public string NouveauMontant = "";
        public int AncienneDureeMois;
        public int NouvelleDureeMois;
        public string Objet = "";
        public string Statut = "";
        public string Observations = "";
    }

    /// <summary>
    /// تمثيل مبسّط لمستخدم داخل النظام.
    /// </summary>
    public class UserItem
    {
        public string Nom = "";
        public string Login = "";
        public string Role = "";
        public string Service = "";
    }

    /// <summary>
    /// تمثيل مبسّط لمتعامل/شريك (مورد، زبون...).
    /// </summary>
    public class PartenaireItem
    {
        public string RaisonSociale = "";
        public string Type = "";
        public string NIF = "";
        public string RC = "";
        public string NIS = "";
        public string Telephone = "";
        public string Email = "";
        public string Adresse = "";
    }

    /// <summary>
    /// مخزن بيانات مؤقت أثناء تشغيل التطبيق (Session-only).
    /// الهدف الحالي: ربط الوحدات ببعضها (Contrats ↔ Avenants ↔ Dashboard)
    /// قبل ربط قاعدة بيانات حقيقية (SQL Server / SQLite) في مرحلة لاحقة.
    /// </summary>
    public static class AppData
    {
        public static readonly List<ContratItem> Contrats = new();
        public static readonly List<AvenantItem> Avenants = new();
        public static readonly List<UserItem> Users = new();
        public static readonly List<PartenaireItem> Partenaires = new();
    }
}
