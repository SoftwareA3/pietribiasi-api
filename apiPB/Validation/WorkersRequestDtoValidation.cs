using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Request;

namespace apiPB.Validation
{
    public static class WorkersRequestDtoValidation
    {
        // Validazione per richiesta WorkerId e Password
        public static WorkersRequestDto ValidateIdAndPasswordRequestDto(this WorkersRequestDto requestDto)
        {
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.WorkerId), nameof(requestDto.Password));
            return requestDto;
        }

        public static WorkersRequestDto ValidatePasswordRequestDto(this WorkersRequestDto requestDto)
        {
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.Password));
            return requestDto;
        }

        public static WorkersFieldRequestDto ValidateIdAndValueRequestDto(this WorkersFieldRequestDto requestDto)
        {
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.WorkerId), nameof(requestDto.FieldValue));
            return requestDto;
        }

        public static WorkersFieldRequestDto ValidateIdRequestDto(this WorkersFieldRequestDto requestDto)
        {
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.WorkerId));
            return requestDto;
        }

        public static WorkersRequestDto ValidateIdRequestDto(this WorkersRequestDto requestDto)
        {
            GenericValidation.ValidateOnlyAllowedProperties(requestDto, nameof(requestDto.WorkerId));
            return requestDto;
        }
    }
}