﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pijaca
{
    public class Tržnica
    {
        #region Atributi

        List<Prodavač> prodavači;
        List<Štand> štandovi;
        double ukupniPrometPijace;

        #endregion

        #region Properties

        public List<Prodavač> Prodavači { get => prodavači; set => prodavači = value; }
        public List<Štand> Štandovi { get => štandovi; }
        public double UkupniPrometPijace { get => ukupniPrometPijace; }

        #endregion

        #region Konstruktor

        public Tržnica()
        {
            prodavači = new List<Prodavač>();
            štandovi = new List<Štand>();
            ukupniPrometPijace = 0.0;
        }

        #endregion

        #region Metode

        public void RadSaProdavačima(Prodavač p, string opcija, double najmanjiPromet)
        {
            if (p == null)
                throw new ArgumentNullException("Morate unijeti informacije o prodavaču!");

            Prodavač postojeći = null;
            foreach (var prodavač in prodavači)
            {
                if (prodavač.Ime == p.Ime)
                {
                    if (p.UkupniPromet < najmanjiPromet)
                        continue;
                    else if (prodavač.UkupniPromet < najmanjiPromet)
                        continue;
                    else if (prodavač.UkupniPromet == p.UkupniPromet)
                        postojeći = prodavač;
                }
            }
            if (opcija == "Dodavanje")
            {
                if (prodavači.FindAll(prod => prod.Ime == p.Ime).Count > 0)
                    throw new InvalidOperationException("Nemoguće dodati prodavača kad već postoji registrovan!");
                else
                    prodavači.Add(p);
            }
            else if (opcija == "Izmjena" || opcija == "Brisanje")
            {
                if (postojeći == null || prodavači.FindAll(prod => prod.Ime == p.Ime).Count == 0)
                    throw new FormatException("Nemoguće izmijeniti tj. obrisati prodavača koji nije registrovan!");
                else
                {
                    prodavači.Remove(postojeći);
                    if (opcija == "Izmjena")
                        prodavači.Add(p);
                }
            }
            else
                throw new InvalidOperationException("Unijeli ste nepoznatu opciju!");
        } 

        public void OtvoriŠtand(Prodavač p, List<Proizvod> pr, DateTime rok)
        {
            if (!prodavači.Contains(p))
                throw new ArgumentException("Prodavač nije registrovan!");
            if (štandovi.Find(š => š.Prodavač == p) != null)
                throw new FormatException("Prodavač može imati samo jedan štand!");
            Štand štand = new Štand(p, rok, pr);
            štandovi.Add(štand);
        }

        public void IzvršavanjeKupovina(Štand š, List<Kupovina> kupovine, string sigurnosniKod)
        {
            Štand štand = štandovi.Find(št => št.Prodavač == š.Prodavač);
            if (štand == null)
                throw new ArgumentException("Unijeli ste štand koji nije registrovan!");

            DateTime najranijaKupovina = kupovine[0].DatumKupovine, najkasnijaKupovina = kupovine[0].DatumKupovine;
            double ukupanPromet = 0;

            foreach (var kupovina in kupovine)
            {
                if (kupovina.DatumKupovine < najranijaKupovina)
                    najranijaKupovina = kupovina.DatumKupovine;
                if (kupovina.DatumKupovine > najkasnijaKupovina)
                    najkasnijaKupovina = kupovina.DatumKupovine;
                ukupanPromet += kupovina.UkupnaCijena;

                štand.RegistrujKupovinu(kupovina);
            }

            Prodavač prodavač = štand.Prodavač;
            prodavač.RegistrujPromet(sigurnosniKod, ukupanPromet, najranijaKupovina, najkasnijaKupovina);
        } 
        public void NaručiProizvode(Štand štand, List<Proizvod> proizvodi, List<int> količine, List<DateTime> rokovi, bool svi = false)
        {
            if (proizvodi.Count != količine.Count || proizvodi.Count != rokovi.Count)
                throw new ArgumentException("Pogrešan unos parametara!");

            for (int i = 0; i < proizvodi.Count; i++)
            {
                if (!svi)
                {
                    Proizvod pr = štand.Proizvodi.Find(p => p.ŠifraProizvoda == proizvodi[i].ŠifraProizvoda);
                    if (pr == null)
                        throw new ArgumentException("Nemoguće naručiti proizvod - nije registrovan na štandu!");

                    pr.NaručiKoličinu(količine[i], rokovi[i]);
                }
                else
                    continue;
            }
        }
        //Radio Hamza Isić
        public void NaručiProizvodeRefaktoring(Štand štand, List<Proizvod> proizvodi, List<int> količine, List<DateTime> rokovi, bool svi = false)
        {
            if (proizvodi.Count != količine.Count || proizvodi.Count != rokovi.Count)
                throw new ArgumentException("Pogrešan unos parametara!");
            if (svi) return;
            int broj = 0;
            proizvodi.ForEach(pr =>
            {
              
                Proizvod proizvodRef = štand.Proizvodi.Find(p => p.ŠifraProizvoda == pr.ŠifraProizvoda);
                if (proizvodRef == null)
                    throw new ArgumentException("Nemoguće naručiti proizvod - nije registrovan na štandu!");

                proizvodRef.NaručiKoličinu(količine[broj], rokovi[broj]);
                broj++;
            });
        }
        //Radio Adnan Hodžić
        public void RadSaProdavačimaRefaktoring(Prodavač p, IOpcije opcija, double najmanjiPromet)
        {
            if (p == null)
                throw new ArgumentNullException("Morate unijeti informacije o prodavaču!");

            Prodavač postojeći = null;


            foreach (var prodavač in prodavači)
                if (prodavač.Ime == p.Ime)
                    if (p.UkupniPromet < najmanjiPromet || prodavač.UkupniPromet < najmanjiPromet)
                        return;
                    else if (prodavač.UkupniPromet == p.UkupniPromet)
                        postojeći = prodavač;

            if (opcija is null)
                throw new InvalidOperationException("Unijeli ste nepoznatu opciju!");

            opcija.izvrsiIzmjenu( prodavači,  p,  postojeći);
        }
        //Adnan
        public void RadSaProdavačimaTuning1(Prodavač p, string opcija, double najmanjiPromet)
        {
            if (p == null)
                throw new ArgumentNullException("Morate unijeti informacije o prodavaču!");

            Prodavač postojeći = null;
            foreach (var prodavač in prodavači)
            {

                if (prodavač.Ime == p.Ime)
                {
                    if (p.UkupniPromet < najmanjiPromet || prodavač.UkupniPromet < najmanjiPromet)
                        continue;
                    else if (prodavač.UkupniPromet == p.UkupniPromet)
                    {
                        postojeći = prodavač;
                        break;
                    }

                }
            }


            if (opcija == "Dodavanje")
            {
                if (prodavači.FindAll(prod => prod.Ime == p.Ime).Count > 0)
                    throw new InvalidOperationException("Nemoguće dodati prodavača kad već postoji registrovan!");
                else
                    prodavači.Add(p);
            }
            else if (opcija == "Izmjena" || opcija == "Brisanje")
            {
                if (postojeći == null || prodavači.FindAll(prod => prod.Ime == p.Ime).Count == 0)
                    throw new FormatException("Nemoguće izmijeniti tj. obrisati prodavača koji nije registrovan!");
                else
                {
                    prodavači.Remove(postojeći);
                    if (opcija == "Izmjena")
                        prodavači.Add(p);
                }
            }
            else
                throw new InvalidOperationException("Unijeli ste nepoznatu opciju!");
        }

        //Adnan i Hamza
        public void RadSaProdavačimaTuning2(Prodavač p, string opcija, double najmanjiPromet)
        {
            if (p == null)
                throw new ArgumentNullException("Morate unijeti informacije o prodavaču!");

            Prodavač postojeći = null;
            foreach (var prodavač in prodavači)
            {
                if (prodavač.Ime == p.Ime)
                {
                    if (p.UkupniPromet < najmanjiPromet && prodavač.UkupniPromet < najmanjiPromet)
                        continue;
                    else if (prodavač.UkupniPromet == p.UkupniPromet)
                    {
                        postojeći = prodavač;
                        break;
                    }

                }
            }
            bool pronađenoIme = Convert.ToBoolean(prodavači.FindAll(prod => prod.Ime == p.Ime).Count);
            if (opcija == "Dodavanje")
            {
                if (pronađenoIme)
                    throw new InvalidOperationException("Nemoguće dodati prodavača kad već postoji registrovan!");
                else
                    prodavači.Add(p);
            }
            else if (opcija == "Izmjena" || opcija == "Brisanje")
            {
                if (postojeći == null || !pronađenoIme)
                    throw new FormatException("Nemoguće izmijeniti tj. obrisati prodavača koji nije registrovan!");
                else
                {
                    prodavači.Remove(postojeći);
                    if (opcija == "Izmjena")
                        prodavači.Add(p);
                }
            }
            else
                throw new InvalidOperationException("Unijeli ste nepoznatu opciju!");
        }

        public void RadSaProdavačimaTuning3(Prodavač p, string opcija, double najmanjiPromet)
        {
            if (p == null)
                throw new ArgumentNullException("Morate unijeti informacije o prodavaču!");

            Prodavač postojeći = null;


            for(int i = 0; i<prodavači.Count; i += 4)
            {
                if (prodavači[i].Ime == p.Ime || prodavači[i+1].Ime == p.Ime || prodavači[i+2].Ime == p.Ime || prodavači[i+3].Ime == p.Ime)
                {
                    if (p.UkupniPromet < najmanjiPromet && prodavači[i].UkupniPromet < najmanjiPromet && prodavači[i+1].UkupniPromet < najmanjiPromet && prodavači[i+2].UkupniPromet < najmanjiPromet && prodavači[i+3].UkupniPromet < najmanjiPromet)
                        continue;
                    else if (prodavači[i].UkupniPromet == p.UkupniPromet)
                    {
                        postojeći = prodavači[i];
                        break;
                    }
                    else if (prodavači[i+1].UkupniPromet == p.UkupniPromet)
                    {
                        postojeći = prodavači[i+1];
                        break;
                    }
                    else if (prodavači[i+2].UkupniPromet == p.UkupniPromet)
                    {
                        postojeći = prodavači[i+2];
                        break;
                    }
                    else if (prodavači[i+3].UkupniPromet == p.UkupniPromet)
                    {
                        postojeći = prodavači[i+3];
                        break;
                    }

                }

            }

            bool pronađenoIme = Convert.ToBoolean(prodavači.FindAll(prod => prod.Ime == p.Ime).Count);
            if (opcija == "Dodavanje")
            {
                if (pronađenoIme)
                    throw new InvalidOperationException("Nemoguće dodati prodavača kad već postoji registrovan!");
                else
                    prodavači.Add(p);
            }
            else if (opcija == "Izmjena" || opcija == "Brisanje")
            {
                if (postojeći == null || !pronađenoIme)
                    throw new FormatException("Nemoguće izmijeniti tj. obrisati prodavača koji nije registrovan!");
                else
                {
                    prodavači.Remove(postojeći);
                    if (opcija == "Izmjena")
                        prodavači.Add(p);
                }
            }
            else
                throw new InvalidOperationException("Unijeli ste nepoznatu opciju!");
        }
        #endregion
    }
}
