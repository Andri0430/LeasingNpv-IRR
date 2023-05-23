using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Diagnostics;
using TopikKhususSIM.Models;

namespace TopikKhususSIM.Controllers
{
    public class HomeController : Controller
    {
        static double[] interest;
        static double[] pvTotal;
        static double[] persen;
        static double[,] pv;
        static double irr;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int hargaBarang,int dpBarang,int cicilan,int option, double[] rasio)
        {
            int bulan = 11;
            interest = new double[option];
            persen = rasio;
            pv = new double[option, bulan];
            pvTotal = new double[option];

            double[] npv = new double[option];

            for (int a = 0; a < option; a++)
            {
                interest[a] = Convert.ToDouble((rasio[a] / 100) / (bulan + 1));
            }

            for (int a = 0; a < option; a++)
            {
                for (int b = 0; b < bulan; b++)
                {
                    pv[a, b] = Convert.ToDouble(cicilan / Math.Pow((1 + interest[a]), b += 1));
                    b -= 1;
                    pvTotal[a] += pv[a, b];
                }
            }

            for (int a = 0; a < option; a++)
            {
                npv[a] = pvTotal[a] - (hargaBarang - dpBarang);
            }

            irr = MenghitungIRR(hargaBarang, dpBarang, cicilan);

            return RedirectToAction("DataNPV", new
            {
                rasio = rasio,
                npv = npv
            });
        }

        public IActionResult DataNPV(double[] rasio, double[] npv)
        {
            int count = 1;
            List<NpvDto> npvDto = new List<NpvDto>();
            for (int a = 0; a < npv.Length; a++)
            {
                npv[a] = Math.Round(npv[a], 5);
                npvDto.Add(new NpvDto
                {
                    No = count++,
                    Npv = npv[a],
                    Rasio = rasio[a]
                });
            }
            ViewBag.NpvData = npvDto;
            ViewBag.IrrDataRound = Math.Round(irr);
            ViewBag.IrrDataDecimal = irr.ToString("G17");
            return View();
        }

        public IActionResult DetailPV(int Id)
        {
            double[] dataPv = new double[11];
            ViewBag.valueInterest = Math.Round(interest[Id - 1], 4);
            ViewBag.valuePvTotal = Convert.ToInt32(pvTotal[Id-1]).ToString("N0");
            ViewBag.valuePersen = persen[Id - 1];

            for(int a = Id-1; a <= Id-1; a++)
            {
                for(int b = 0; b < 11; b++)
                {
                    dataPv[b] = Convert.ToInt32(pv[a, b]);
                }
            }

            ViewBag.valueDataPv = dataPv;
            return View();
        }

        public double MenghitungIRR(int harga, int dp, int cicilan)
        {
            bool z = true;
            int bulan = 11;
            double[] pv = new double[11];
            double pvTotal = 0, npv = 0, interest = 0, rasio = 0;

            while (z)
            {
                rasio += 0.00001;
                interest = Convert.ToDouble((rasio / 100) / (bulan + 1));

                for (int a = 0; a < bulan; a++)
                {
                    pv[a] = Convert.ToDouble(cicilan / Math.Pow((1 + interest), a += 1));
                    a -= 1;
                    pvTotal += pv[a];
                }

                npv = pvTotal - (harga - dp);

                if (Convert.ToInt32(npv) <= 0)
                {
                    z = false;
                }
                pvTotal = 0;
            }
            return rasio;
        }
    }
}