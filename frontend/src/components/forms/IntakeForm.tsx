"use client";

import { useForm } from "react-hook-form";
import InputField from "../InputField";
import { Dispatch, SetStateAction, useEffect, useRef, useState } from "react";
import { toast } from "react-toastify";
import { effect, promise } from "zod";
import axios from "axios";

const IntakeForm = ({
  type,
  data,
  setOpen,
}: {
  type: "create" | "update";
  data?: any;
  setOpen: Dispatch<SetStateAction<boolean>>;
  relatedData?: any;
}) => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const [state, setState] = useState({ success: false, error: false });
  const [initLookup , setInitLookup]  = useState({
    isLoading : true ,
    leadSources : [] as Array<{ id: number; name: string }>, 
    genders : [] as Array<{ id: number; name: string }>
  });

  
  useEffect(() => {
      const fetchInit = async  () => {
          const [res , res2]  = await Promise.allSettled([
               await axios.get('api/leadSources'),
               await axios.get('api/genders')
          ]);


          if(res.status == "fulfilled" && res2.status == "fulfilled")
          {
            setInitLookup({
              isLoading : false , 
              leadSources : res.status == "fulfilled" ?  res.value.data : [] ,
              genders : res2.status == 'fulfilled' ? res2.value.data : []

            })
          }

      }
      fetchInit()
  }, []);

  const onSubmit = handleSubmit(async (formData) => {
    // Mock submission - in real app, this would call an API
    console.log("Intake form data:", formData);
    setState({ success: true, error: false });
  });

  useEffect(() => {
    if (state.success) {
      toast(`Intake has been ${type === "create" ? "created" : "updated"}!`);
      setOpen(false);
      // redirect to list intakes
    }
  }, [state.success, type, setOpen]);

  const { leadSources = [], opcs = [] } = relatedData ?? {};

  return (
    <div className="flex flex-col gap-8" >
    
      <span className="text-xs text-gray-400 font-medium">
        Personal Information
      </span>
      <div className="flex justify-between flex-wrap gap-4">
        <InputField
          label="First Name"
          name="firstName"
          defaultValue={data?.firstName}
          register={register}
          error={errors?.firstName as any}
        />
        <InputField
          label="Last Name"
          name="lastName"
          defaultValue={data?.lastName}
          register={register}
          error={errors.lastName as any}
        />
        <InputField
          label="Email"
          name="email"
          type="email"
          defaultValue={data?.email}
          register={register}
          error={errors?.email as any}
        />
        <InputField
          label="Phone"
          name="phone"
          defaultValue={data?.phone}
          register={register}
          error={errors.phone as any}
        />
        <InputField
          label="Date of Birth"
          name="dateOfBirth"
          defaultValue={data?.dateOfBirth}
          register={register}
          error={errors.dateOfBirth as any}
          type="date"
        />
        <div className="flex flex-col gap-2 w-full md:w-1/4">
          <label className="text-xs text-gray-500">Gender</label>
          <select
            className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full"
            {...register("gender")}
            defaultValue={data?.gender}
          >
            <option value="Male">Male</option>
            <option value="Female">Female</option>
            <option value="Other">Other</option>
          </select>
          {errors.gender?.message && (
            <p className="text-xs text-red-400">
              {errors.gender.message.toString()}
            </p>
          )}
        </div>
      </div>
      
      <span className="text-xs text-gray-400 font-medium">
        Intake Information
      </span>
      <div className="flex justify-between flex-wrap gap-4">
        <div className="flex flex-col gap-2 w-full md:w-1/4">
          <label className="text-xs text-gray-500">Lead Source</label>
          <select
            className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full"
            {...register("leadSourceId")}
            defaultValue={data?.leadSourceId}
          >
            {leadSources.map((leadSource: { id: number; name: string }) => (
              <option value={leadSource.id} key={leadSource.id}>
                {leadSource.name}
              </option>
            ))}
          </select>
          {errors.leadSourceId?.message && (
            <p className="text-xs text-red-400">
              {errors.leadSourceId.message.toString()}
            </p>
          )}
        </div>
        
        <div className="flex flex-col gap-2 w-full md:w-1/4">
          <label className="text-xs text-gray-500">OPC</label>
          <select
            className="ring-[1.5px] ring-gray-300 p-2 rounded-md text-sm w-full"
            {...register("opcId")}
            defaultValue={data?.opcId}
          >
            {opcs.map((opc: { id: number; name: string }) => (
              <option value={opc.id} key={opc.id}>
                {opc.name}
              </option>
            ))}
          </select>
          {errors.opcId?.message && (
            <p className="text-xs text-red-400">
              {errors.opcId.message.toString()}
            </p>
          )}
        </div>
        
        <InputField
          label="Intake Date"
          name="intakeDate"
          defaultValue={data?.intakeDate}
          register={register}
          error={errors.intakeDate as any}
          type="date"
        />
        
        {data && (
          <InputField
            label="Id"
            name="id"
            defaultValue={data?.id}
            register={register}
            error={errors?.id as any}
            hidden
          />
        )}
      </div>
      
      {state.error && (
        <span className="text-red-500">Something went wrong!</span>
      )}
      <button 
       onClick={(e) => {
           e.preventDefault() ;
           onSubmit()
       }}
       type="button" className="bg-blue-400 text-white p-2 rounded-md">
        {type === "create" ? "Create" : "Update"}
      </button>
    </div>
  );
};

export default IntakeForm;
