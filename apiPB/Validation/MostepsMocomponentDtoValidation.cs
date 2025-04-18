using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Request;

namespace apiPB.Validation
{
    public static class MostepsMocomponentDtoValidation
    {
        // Validazione per richiesta Job
        public static MostepsMocomponentRequestDto ValidateJobRequestDto(this MostepsMocomponentRequestDto requestDto)
        {
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Job));
            return requestDto;
        }

        // Validazione per richiesta Mono che include Job
        public static MostepsMocomponentRequestDto ValidateMonoRequestDto(this MostepsMocomponentRequestDto requestDto)
        {   
            if (string.IsNullOrEmpty(requestDto.Mono))
            {
                throw new ArgumentException("Mono cannot be null or empty.");
            }
            
            if (requestDto.CreationDate == null)
            {
                throw new ArgumentException("CreationDate cannot be null.");
            }
            
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Job), nameof(requestDto.Mono), nameof(requestDto.CreationDate));
            return requestDto;
        }

        // Validazione per richiesta Operation che include Job, Mono e CreationDate
        public static MostepsMocomponentRequestDto ValidateOperationRequestDto(this MostepsMocomponentRequestDto requestDto)
        {   
            if (string.IsNullOrEmpty(requestDto.Operation))
            {
                throw new ArgumentException("Operation cannot be null or empty.");
            }
            
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Job), nameof(requestDto.Mono), nameof(requestDto.CreationDate), nameof(requestDto.Operation));
            return requestDto;
        }

        public static MostepsMocomponentRequestDto ValidateBarCodeRequestDto(this MostepsMocomponentRequestDto requestDto)
        {   
            if (string.IsNullOrEmpty(requestDto.BarCode))
            {
                throw new ArgumentException("BarCode cannot be null or empty.");
            }
            
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Job), nameof(requestDto.Mono), nameof(requestDto.CreationDate), nameof(requestDto.Operation), nameof(requestDto.BarCode));
            return requestDto;
        }
    }
}