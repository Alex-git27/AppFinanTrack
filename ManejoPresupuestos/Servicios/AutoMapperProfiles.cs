using AutoMapper;
using ManejoPresupuestos.Models;

namespace ManejoPresupuestos.Servicios
{
    public class AutoMapperProfiles: Profile

    {
        public AutoMapperProfiles()
        { 
            CreateMap<Cuenta, CuentaCreacionViewModel>();
            CreateMap<TransaccionActualizacionViewModel, Transaccion>().ReverseMap(); //El reverse Map funciona para Mapear de manera viceversa, entre si
                                                                                      //Configuramos ambos Mapeos
        }

    }
}
